using ArticleService.Rpc.Protos;
using CommentService.App.Interface;
using CommentService.Domain;
using CommentService.Request;
using CommentService.Response;
using Google.Protobuf.WellKnownTypes;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using static ArticleService.Rpc.Protos.gArticle;
using static CommentService.Rpc.Protos.gUser;

namespace CommentService.App
{
    public class LikeApp : ILikeApp
    {
        private readonly IDbContextFactory<CommentDBContext> contextFactory;
        private readonly gArticleClient articleRpc;
        private readonly gUserClient userRpc;
        private readonly IDatabase cache;
        /// <summary>
        /// 序列化配置
        /// </summary>
        private readonly JsonSerializerSettings jsonConfig;

        public LikeApp(IDbContextFactory<CommentDBContext> contextFactory, IConnectionMultiplexer connection, gArticleClient articleRpc, gUserClient userRpc)
        {
            this.contextFactory = contextFactory;
            this.articleRpc = articleRpc;
            this.userRpc = userRpc;
            this.cache = connection.GetDatabase(2);
            this.jsonConfig = new JsonSerializerSettings();
            this.jsonConfig.ContractResolver = new CamelCasePropertyNamesContractResolver(); //启用小驼峰格式
            this.jsonConfig.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }

        /// <summary>
        /// 点赞/收藏(取消)
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="uid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<string> LikeArticle(int aid, int uid, int status)
        {
            try
            {
                using (var dbContext = contextFactory.CreateDbContext())
                {
                    var like = await dbContext.Likes.FirstOrDefaultAsync(l => l.ArticleId == aid && l.UserId == uid);
                    if (like != null)
                    {
                        if(like.Status >= status || like.Status == 3)
                        {
                            //原有状态值如果等于当前状态值，则说明将状态降级
                            like.Status -= status;
                        }
                        else
                        {
                            //提升状态
                            like.Status += status;
                        }

                        //点赞状态为负值，则删除点赞记录
                        if(like.Status <= 0)
                        {
                            dbContext.Likes.Remove(like);
                        }
                        else
                        {
                            dbContext.Likes.Update(like);
                        }
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        like = new Like()
                        {
                            ArticleId = aid,
                            UserId = uid,
                            Status = status
                        };

                        dbContext.Likes.Add(like);
                        if((await dbContext.SaveChangesAsync()) > 0)
                        {
                            var articleInfo = await articleRpc.GetArticleInfoAsync(new ArticleId() { Id = aid });
                            if (articleInfo.UserId != uid)
                            {
                                //通知对方
                                await cache.StringSetAsync($"likeNotify:{articleInfo.UserId}:{like.Id}", JsonConvert.SerializeObject(like, jsonConfig));
                            }
                        }
                    }
                }
                return "点赞成功";
            }
            catch (Exception)
            {
                throw new Exception("点赞失败");
            }
        }

        /// <summary>
        /// 获取用户的点赞/收藏记录
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<LikeView>> GetUserLike(int uid,bool isLike)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //调用rpc获取用户列表
                var userList = (await userRpc.GetUserListAsync(new Empty())).UserInfo.ToList();
                //调用rpc获取文章列表
                var articleList = (await articleRpc.GetArticleListAsync(new Empty())).Infos.ToList();
                var likes = await dbContext.Likes.Where(l => l.UserId == uid).ToListAsync();
                var likeView = (from l in likes
                                join u in userList on l.UserId equals u.Id
                                join a in articleList on l.ArticleId equals a.Id
                                where l.UserId == uid && isLike ? (l.Status == 1 || l.Status == 3) : (l.Status == 2 || l.Status == 3)
                                select new
                                {
                                    Id = l.Id,
                                    ArticleId = l.ArticleId,
                                    ArticleTitle = a.Title,
                                    UserId = l.UserId,
                                    Status = l.Status,
                                    Nick = u.Nick,
                                    Photo = u.Photo,
                                    Username = u.Username,
                                }).ToList();
                return likeView.MapToList<LikeView>();
            }
        }

        /// <summary>
        /// 获取用户给我点赞得信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<LikeView>> GetUserLikeMessage(int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                List<LikeView> likeListViews = new List<LikeView>();
                // 获取所有未读评论缓存键
                var likeKeys = await cache.ScriptEvaluateAsync(LuaScript.Prepare($"local res = redis.call('KEYS','likeNotify:{uid}*') return res"));
                if (!likeKeys.IsNull)
                {
                    //调用rpc获取用户列表
                    var userList = (await userRpc.GetUserListAsync(new Empty())).UserInfo.ToList();
                    // 调用rpc获取用户的文章列表
                    var articleList = (await articleRpc.GetArticleListAsync(new Empty())).Infos.ToList();
                    //获取所有的值
                    RedisKey[] keys = (RedisKey[])likeKeys;
                    var values = await cache.StringGetAsync(keys);
                    foreach (var item in values)
                    {
                        //将值序列化为对象
                        var like = JsonConvert.DeserializeObject<Like>(item);
                        var user = userList.FirstOrDefault(u => u.Id == like.UserId);
                        var article = articleList.FirstOrDefault(a => a.Id == like.ArticleId);
                        if (article != null && user != null)
                        {
                            likeListViews.Add(new LikeView
                            {
                                Id = like.Id,
                                ArticleId = article.Id,
                                ArticleTitle = article.Title,
                                UserId = user.Id,
                                Username = user.Username,
                                Nick = user.Nick,
                                Photo = user.Photo,
                                Status = like.Status,
                            });
                        }
                    }
                }
                return likeListViews;
            }
        }

        /// <summary>
        /// 获取文章的点赞/收藏数
        /// </summary>
        /// <param name="aid">文章ID</param>
        /// <param name="status">1点赞数2收藏数</param>
        /// <returns></returns>
        public async Task<int> GetArticleLikeCount(int aid,int status)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var count = await dbContext.Likes.CountAsync(l => l.ArticleId == aid && l.Status == status);
                return count;
            }
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="uid"></param>
        /// <param name="isAll"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> DeleteLikeMessage(int uid, int lid, bool isAll = false)
        {
            //已读全部
            if (isAll)
            {
                var commentKeys = await cache.ScriptEvaluateAsync(LuaScript.Prepare($"local res = redis.call('KEYS','likeNotify:{uid}*') return res"));
                if (!commentKeys.IsNull)
                {
                    RedisKey[] k = (RedisKey[])commentKeys;
                    await this.cache.KeyDeleteAsync(k);
                }
            }
            else
            {
                await cache.KeyDeleteAsync($"likeNotify:{uid}:{lid}");
            }

            return "操作成功";
        }
    }
}
