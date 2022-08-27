namespace CommentService.Request
{
    public class LikeArticleReq
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        public int ArticleId { get; set; }
        /// <summary>
        /// 0点赞1收藏
        /// </summary>
        public int Status { get; set; }
    }
}
