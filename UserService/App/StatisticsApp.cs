using Microsoft.EntityFrameworkCore;
using UserService.App.Interface;
using UserService.Response;

namespace UserService.App
{
    public class StatisticsApp:IStatisticsApp
    {
        private readonly IDbContextFactory<UserDBContext> contextFactory;

        public StatisticsApp(IDbContextFactory<UserDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// 统计用户
        /// </summary>
        /// <returns></returns>
        public async Task<UserCountStatistics> GetUserCount()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var nowTime = DateTime.Now;
                var yesterdayTime = nowTime.AddDays(-1);

                var userList = await context.Users.Include(u => u.UserDetail).ToListAsync();
                var count = new UserCountStatistics();
                count.UserCount = userList.Count;
                count.EnableCount = userList.Where(u => u.Status == 1).Count();
                count.DisableCount = userList.Where(u => u.Status == -1).Count();
                count.TodayCount = userList.Where(u => u.UserDetail?.RegisterTime?.Date == nowTime.Date).Count();
                count.YesterdayCount = userList.Where(u => u.UserDetail?.RegisterTime?.Date == yesterdayTime.Date).Count();
                count.WeekCount = userList.Where(u => u.UserDetail?.RegisterTime?.DayOfWeek == nowTime.DayOfWeek).Count();
                count.MonthCount = userList.Where(u => u.UserDetail?.RegisterTime?.Month == nowTime.Month).Count();

                return count;
            }
        }

        
        /// <summary>
        /// 统计用户趋势
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserTrendStatistics>> GetUserTrend()
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //获取所有数据
                var userList = await dbContext.UserDetails.OrderBy(u => u.RegisterTime).ToListAsync();
                var trends = userList.GroupBy(u => u.RegisterTime.Value.Date).Select(ug => new UserTrendStatistics
                {
                    Date = ug.Key,
                    UserCount = ug.Count(),
                });
                return trends.ToList();
            }
        }
    }
}
