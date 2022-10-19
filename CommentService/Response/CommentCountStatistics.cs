namespace CommentService.Response
{
    public class CommentCountStatistics
    {
        public int CommentCount { get; set; }
        public int PenddingCount { get; set; }
        public int TodayCount { get; set; }
        public int YesterdayCount { get; set; }
        public int WeekCount { get; set; }
        public int MonthCount { get; set; }
    }
}
