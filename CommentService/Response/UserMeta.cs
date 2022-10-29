namespace CommentService.Response
{
    public class UserMeta
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
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
    }
}
