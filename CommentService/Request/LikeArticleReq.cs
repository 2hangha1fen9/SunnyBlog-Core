namespace CommentService.Request
{
    public class LikeArticleReq
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        public int ArticleId { get; set; }
        /// <summary>
        /// 1点赞2收藏
        /// </summary>
        public int Status { get; set; }
    }
}
