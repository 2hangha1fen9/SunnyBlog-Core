namespace CommentService.Response
{
    public class Meta
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        public int ArticleId { get; set; }
        /// <summary>
        /// 浏览量
        /// </summary>
        public long ViewCount { get; set; }
        /// <summary>
        /// 点赞量
        /// </summary>
        public long LikeCount { get; set; }
        /// <summary>
        /// 评论量
        /// </summary>
        public long CommentCount { get; set; }
        /// <summary>
        /// 收藏量
        /// </summary>
        public long CollectionCount { get; set; }
        /// <summary>
        /// 当前用户点赞
        /// </summary>
        public long? IsUserLike { get; set; }
        /// <summary>
        /// 当前用户收藏
        /// </summary>
        public long? IsUserCollection { get; set; }
    }
}
