using CommentService.App.Interface;
using CommentService.Response;

namespace CommentService.App
{
    public class CountApp : ICountApp
    {
        private readonly ICommentApp commentApp;
        private readonly ILikeApp likeApp;
        private readonly IViewApp viewApp;

        public CountApp(ICommentApp commentApp, ILikeApp likeApp, IViewApp viewApp)
        {
            this.commentApp = commentApp;
            this.likeApp = likeApp;
            this.viewApp = viewApp;
        }

        /// <summary>
        /// 统计文章数据
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<ArticleCountView> GetArticleCount(int aid,int? uid = null)
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
                var countView = new ArticleCountView()
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
    }
}
