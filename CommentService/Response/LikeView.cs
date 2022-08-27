namespace CommentService.Response
{
    public class LikeView
    {
        /// <summary>
        /// 喜欢/收藏ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 喜欢/收藏文章ID
        /// </summary>
        public int ArticleId { get; set; }
        /// <summary>
        /// 喜欢/收藏用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 0点赞1收藏
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string ArticleTitle { get; set; }
    }
}
