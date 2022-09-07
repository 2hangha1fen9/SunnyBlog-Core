namespace ArticleService.Response
{
    /// <summary>
    /// 分区视图
    /// </summary>
    public class RegionView
    {
        /// <summary>
        /// 分区ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 分区名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分区描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 分区状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 子级分类
        /// </summary>
        public List<RegionView> InverseParent { get; set; }
    }
}
