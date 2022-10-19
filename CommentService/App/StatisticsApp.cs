using CommentService.App.Interface;
using CommentService.Response;
using Microsoft.EntityFrameworkCore;

namespace CommentService.App
{
    public class StatisticsApp: IStatisticsApp
    {
        private readonly IDbContextFactory<CommentDBContext> contextFactory;

        public StatisticsApp(IDbContextFactory<CommentDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<CommentCountStatistics> GetCommentCount(int? uid = null)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var nowTime = DateTime.Now;
                var yesterdayTime = nowTime.AddDays(-1);

                //查询所有数据
                var commentList = await dbContext.Comments.ToListAsync();
                if (uid.HasValue)
                {
                    commentList = commentList.Where(c => c.UserId == uid.Value).ToList();
                }

                var count = new CommentCountStatistics();
                //统计数据
                count.CommentCount = commentList.Count;
                count.PenddingCount = commentList.Where(c => c.Status == 2).Count();
                count.TodayCount = commentList.Where(c => c.CreateTime.Date == nowTime.Date).Count();
                count.YesterdayCount = commentList.Where(c => c.CreateTime.Date == yesterdayTime.Date).Count();
                count.WeekCount = commentList.Where(c => c.CreateTime.DayOfWeek == nowTime.DayOfWeek).Count();
                count.MonthCount = commentList.Where(c => c.CreateTime.Month == nowTime.Month).Count();

                return count;
            }
        }
    }
}
