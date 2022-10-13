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
        public int ViewCount { get; set; }
        /// <summary>
        /// 点赞量
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 评论量
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 收藏量
        /// </summary>
        public int CollectionCount { get; set; }
    }
}
