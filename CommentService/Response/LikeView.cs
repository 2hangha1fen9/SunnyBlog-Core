namespace CommentService.Response
{
    public class LikeView
    {
        /// <summary>
        /// 点赞id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 喜欢/收藏文章ID
        /// </summary>
        public int ArticleId { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string? ArticleTitle { get; set; }
        /// <summary>
        /// 喜欢/收藏用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nick { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string Photo { get; set; }
        /// <summary>
        /// 1点赞2收藏
        /// </summary>
        public int Status { get; set; }
    }
}
