namespace CommentService.Response
{
    public class LikeView
    {
        /// <summary>
        /// 喜欢/收藏文章ID
        /// </summary>
        public int ArticleId { get; set; }
        /// <summary>
        /// 喜欢/收藏用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 1点赞2收藏
        /// </summary>
        public int Status { get; set; }
    }
}
