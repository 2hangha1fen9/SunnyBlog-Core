namespace ArticleService.Response
{
    public class TagView
    {
        /// <summary>
        /// 标签ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标签颜色
        /// </summary>
        public string Color { get; set; }
    }
}
