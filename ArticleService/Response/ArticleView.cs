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
        /// 文章标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 文章简介
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 文章内容摘要
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 文章封面
        /// </summary>
        public string Photo { get; set; }
        /// <summary>
        /// 文章分区名
        /// </summary>
        public string RegionName { get; set; }
        /// <summary>
        /// 文章分区Id
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
