namespace CommentService.Response
{
    public class UserViewHistory
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// 文章ID
        /// </summary>
        public int ArticleId { get; set; }
        /// <summary>
        /// 用户IP
        /// </summary>
        public string? Ip { get; set; }
        /// <summary>
        /// 访问时间
        /// </summary>
        public DateTime ViewTime { get; set; }
    }
}
