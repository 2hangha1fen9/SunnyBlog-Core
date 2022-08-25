namespace ArticleService.Response
{
    public class CategoryView
    {
        /// <summary>
        /// 分类ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 父级分类ID
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 子级分类
        /// </summary>
        public List<CategoryView> InverseParent { get; set; }
    }
}
