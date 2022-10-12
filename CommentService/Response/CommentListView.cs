namespace CommentService.Response
{
    public class CommentListView
    {
        /// <summary>
        /// 评论ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 文章ID
        /// </summary>
        public int ArticleId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string ArticleTitle { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Nick { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string? Photo { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 评论状态 1审核通过0未审核
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 发表时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
