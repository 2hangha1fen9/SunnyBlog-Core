namespace ArticleService.Response
{
    /// <summary>
    /// 文章列表展示视图
    /// </summary>
    public class ArticleListView
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
        /// 用户昵称
        /// </summary>
        public string Nick { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 文章简介
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 文章封面
        /// </summary>
        public string Photo { get; set; }
        /// <summary>
        /// 分区名称
        /// </summary>
        public string RegionName { get; set; }
        /// <summary>
        /// 分区ID
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// 文章标签
        /// </summary>
        public List<TagView> Tags { get; set; }
        /// <summary>
        /// 文章分类
        /// </summary>
        public List<CategoryView> Categorys { get; set; }
        /// <summary>
        /// 文章状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 是否被锁定
        /// </summary>
        public int IsLock { get; set; }
        /// <summary>
        /// 评论状态
        /// </summary>
        public int CommentStatus { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
