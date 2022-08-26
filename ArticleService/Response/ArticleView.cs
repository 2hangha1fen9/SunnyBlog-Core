namespace ArticleService.Response
{
    /// <summary>
    /// 文章详情展示视图
    /// </summary>
    public class ArticleView
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserPhoto { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 文章内容摘要
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 文章封面
        /// </summary>
        public string Photo { get; set; }
        /// <summary>
        /// 文章标签
        /// </summary>
        public List<TagView> Tags { get; set; }
        /// <summary>
        /// 文章分区名
        /// </summary>
        public string RegionName { get; set; }
        /// <summary>
        /// 文章分区Id
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Nick { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 文章状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 评论状态图
        /// </summary>
        public int CommentStatus { get; set; }
    }
}
