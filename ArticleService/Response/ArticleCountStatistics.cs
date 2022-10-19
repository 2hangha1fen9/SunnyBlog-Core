namespace ArticleService.Response
{
    public class ArticleCountStatistics
    {
        /// <summary>
        /// 文章总数
        /// </summary>
        public int ArticleCount { get; set; }
        /// <summary>
        /// 标签数
        /// </summary>
        public int TagCount { get; set; }
        /// <summary>
        /// 展示文章总数
        /// </summary>
        public int ShowArticleCount { get; set; }
        /// <summary>
        /// 待审核文章数
        /// </summary>
        public int PenddingArticleCount { get; set; }
        /// <summary>
        /// 锁定的文章数
        /// </summary>
        public int LockArticleCount { get; set; }
        /// <summary>
        /// 回收站的文章数
        /// </summary>
        public int RecycleArticleCount { get; set; }
        /// <summary>
        /// 今日发布
        /// </summary>
        public int TodayPublish { get; set; }
        /// <summary>
        /// 昨天发布文章数
        /// </summary>
        public int YesterdayPublish { get; set; }
        /// <summary>
        /// 本周发布文章数
        /// </summary>
        public int WeekPublish { get; set; }
        /// <summary>
        /// 本月发布文章数
        /// </summary>
        public int MonthPublish { get; set; }
    }
}
