namespace ArticleService.Request
{
    public class UpdateCategoryReq
    {
        /// <summary>
        /// 分类ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 分类名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public int ParentId { get; set; }
    }
}
