namespace CommentService.Response
{
    public class ArticleCountView
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        public int ArticleId { get; set; }
        /// <summary>
        /// 浏览量
        /// </summary>
        public int ViewCount { get; set; }
        /// <summary>
        /// 点赞量
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 点赞量
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 收藏量
        /// </summary>
        public int CollectionCount { get; set; }
        /// <summary>
        /// 当前用户点赞
        /// </summary>
        public int? IsUserLike { get; set; }
        /// <summary>
        /// 当前用户收藏
        /// </summary>
        public int? IsUserCollection { get; set; }
    }
}
