using CommentService.App.Interface;
using CommentService.Response;
using Microsoft.EntityFrameworkCore;
using static ArticleService.Rpc.Protos.gArticle;

namespace CommentService.App
{
    public class MetaApp : IMetaApp
    {
        private readonly ILikeApp likeApp;
        private readonly IViewApp viewApp;
        private readonly ICommentApp commentApp;
        private readonly IDbContextFactory<CommentDBContext> contextFactory;

        public MetaApp(ICommentApp commentApp, ILikeApp likeApp, IViewApp viewApp, IDbContextFactory<CommentDBContext> contextFactory)
        {
            this.commentApp = commentApp;
            this.likeApp = likeApp;
            this.viewApp = viewApp;
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// 获取文章元数据
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<Meta> GetMeta(int aid,int? uid = null)
        {
            try
            {
                //统计文章数据
                var commentCount = await commentApp.GetArticleCommentCount(aid);
                var likeCount = await likeApp.GetArticleLikeCount(aid, 1);
                var collectionCount = await likeApp.GetArticleLikeCount(aid, 2);
                var viewCount = await viewApp.GetArticleViewCount(aid);
                var isUserLike = 0;
                var isUserCollection = 0;
                if (uid.HasValue)
                {
                    //获取用户的点赞收藏记录
                    var userLike = await likeApp.GetUserLike(uid.Value);
                    //判断用户是否点赞
                    isUserLike = userLike.FirstOrDefault(u => u.ArticleId == aid && u.Status == 1) != null ? 1 : 0;
                    //判断用户是否收藏
                    isUserCollection = userLike.FirstOrDefault(u => u.ArticleId == aid && u.Status == 2) != null ? 1 : 0;
                }
                var countView = new Meta()
                {
                    ArticleId = aid,
                    CollectionCount = collectionCount,
                    ViewCount = viewCount,
                    CommentCount = commentCount,
                    IsUserLike = isUserLike,
                    IsUserCollection = isUserCollection,
                };
                return countView;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取一批文章互动元数据
        /// </summary>
        /// <param name="aids"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<Meta>> GetMetaList(int[] aids,int?uid = null)
        {
            using (var dbContent = contextFactory.CreateDbContext())
            {
                var commentList = await dbContent.Comments.ToListAsync();
                var likeList = await dbContent.Likes.ToListAsync();
                var viewList = await dbContent.Views.ToListAsync();
                var metas = from a in aids
                            join c in commentList on a equals c.ArticleId into comment
                            join l in likeList on a equals l.ArticleId into like
                            join v in viewList on a equals v.ArticleId into view
                            group new Meta {
                                ArticleId = a,
                                ViewCount = view.Count(vv => vv.ArticleId == a),
                                LikeCount = like.Count(ll => ll.ArticleId == a && ll.Status == 1),
                                CommentCount = comment.Count(cc => cc.ArticleId == a),
                                CollectionCount = like.Count(cc => cc.ArticleId == a && cc.Status == 2),
                                IsUserLike = uid.HasValue && like.FirstOrDefault(ll => ll.ArticleId == a && ll.UserId == uid.Value && ll.Status == 1) != null ? 1 : 0,
                                IsUserCollection = uid.HasValue && like.FirstOrDefault(ll => ll.ArticleId == a && ll.UserId == uid.Value && ll.Status == 2) != null ? 1 : 0,
                            } by a;

                var metaList = new List<Meta>();
                foreach (var meta in metas)
                {
                    foreach (var m in meta)
                    {
                        metaList.Add(m);
                    }
                }
                return metaList;
            }
        }
    }
}
