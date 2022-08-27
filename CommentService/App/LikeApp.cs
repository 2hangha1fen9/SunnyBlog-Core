using CommentService.App.Interface;
using CommentService.Domain;
using CommentService.Request;
using CommentService.Response;
using Google.Protobuf.WellKnownTypes;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        /// 获取用户所有点赞/收藏
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<PageList<LikeView>> GetUserLike(List<SearchCondition> condidtion, int uid, int status, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //获取所有文章,为了获取标题
                var articleList = (await articleRpc.GetArticleListAsync(new Empty())).Infos.ToList();
                var collection = await dbContext.Likes.Where(l => l.UserId == uid && l.Status == status).ToListAsync();
                if (articleList.Count > 0 && collection.Count > 0)
                {
                    var likeView = collection.Join(articleList, c => c.ArticleId, a => a.Id, (c, a) => new
                    {
                        Id = c.Id,
                        ArticleId = a.Id,
                        UserId = a.UserId,
                        Status = c.Status,
                        ArticleTitle = a.Title
                    }).AsQueryable();

                    //条件筛选
                    if (condidtion.Count > 0)
                    {
                        foreach (var con in condidtion)
                        {
                            likeView = con.Key.Contains(con.Value, StringComparison.OrdinalIgnoreCase) ? likeView.Where(l => l.ArticleTitle.Contains(con.Value)) : likeView;
                        }
                    }
                    var page = new PageList<LikeView>();
                    likeView = page.Pagination(pageIndex, pageSize, likeView);
                    page.Page = likeView.MapToList<LikeView>();
                    return page;
                }
                return null;
            }    
        }

        /// <summary>
        /// (取消/添加)点赞/收藏文章
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<string> LikeArticle(int aid,int uid,int status)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                try
                {
                    var like = dbContext.Likes.FirstOrDefault(l => l.ArticleId == aid && l.UserId == uid && l.Status == status);
                    if (like != null)
                    {
                        dbContext.Likes.Remove(like);
                    }
                    else 
                    {
                        like = new Like()
                        {
                            UserId = uid,
                            ArticleId = aid,
                            Status = status
                        };
                        dbContext.Likes.Add(like);
                    }
                    await dbContext.SaveChangesAsync();
                    return "操作成功";
                }
                catch (Exception)
                {
                    throw new Exception("操作失败");
                }
            }
        }

    }
}
