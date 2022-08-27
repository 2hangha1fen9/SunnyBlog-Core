namespace CommentService.Response
{
    /// <summary>
    /// 评论列表视图
    /// </summary>
    public class CommentView
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
        /// 用户昵称
        /// </summary>
        public string Nick { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string Photo { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 发表时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 父级评论ID
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 子级评论
        /// </summary>
        public List<CommentView> InverseParent { get; set; }
    }
}
