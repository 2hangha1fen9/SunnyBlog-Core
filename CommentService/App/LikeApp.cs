using CommentService.App.Interface;
using CommentService.Domain;
using CommentService.Request;
using CommentService.Response;
using Google.Protobuf.WellKnownTypes;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using static ArticleService.Rpc.Protos.gArticle;

namespace CommentService.App
{
    public class LikeApp : ILikeApp
    {
        private readonly IDbContextFactory<CommentDBContext> contextFactory;
        private readonly gArticleClient articleRpc;

        public LikeApp(IDbContextFactory<CommentDBContext> contextFactory, gArticleClient articleRpc)
        {
            this.contextFactory = contextFactory;
            this.articleRpc = articleRpc;
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
                    var like = await dbContext.Likes.FirstOrDefaultAsync(l => l.ArticleId == aid && l.UserId == uid && l.Status == status);
                    if (like != null)
                    {
                        //用户取消点赞
                        dbContext.Likes.Remove(like);
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
                    }
                    await dbContext.SaveChangesAsync();
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
        public async Task<List<LikeView>> GetUserLike(int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var likes = await dbContext.Likes.Where(l => l.UserId == uid).ToListAsync();
                return likes.MapToList<LikeView>();
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
    }
}
