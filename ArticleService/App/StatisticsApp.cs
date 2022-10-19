using ArticleService.App.Interface;
using ArticleService.Response;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.App
{
    public class StatisticsApp : IStatisticsApp
    {
        private readonly IDbContextFactory<ArticleDBContext> contextFactory;

        public StatisticsApp(IDbContextFactory<ArticleDBContext> contextFactory, IArticleTagApp articleTagApp)
        {
            this.contextFactory = contextFactory;
        }


        /// <summary>
        /// 统计文章数
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ArticleCountStatistics> GetArticleCount(int? uid = null)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var nowTime = DateTime.Now;
                var yesterdayTime = nowTime.AddDays(-1);

                //查询所有数据
                var articleList = await dbContext.Articles.ToListAsync();
                var tagList = await dbContext.Tags.ToListAsync();

                if (uid.HasValue)
                {
                    articleList = articleList.Where(x => x.UserId == uid.Value).ToList();
                    tagList = tagList.Where(x => x.UserId == uid.Value).ToList();
                }

                var count = new ArticleCountStatistics();
                //统计数据
                count.ArticleCount = articleList.Count;
                count.ShowArticleCount = articleList.Where(a => a.Status == 1).Count();
                count.PenddingArticleCount = articleList.Where(a => a.Status == -1).Count();
                count.LockArticleCount = articleList.Where(a => a.IsLock == -1).Count();
                count.RecycleArticleCount = articleList.Where(a => a.IsLock == 3).Count();
                count.TagCount = tagList.Count;
                count.TodayPublish = articleList.Where(a => a.CreateTime.Date == nowTime.Date).Count();
                count.YesterdayPublish = articleList.Where(a => a.CreateTime.Date == yesterdayTime.Date).Count();
                count.WeekPublish = articleList.Where(a => a.CreateTime.DayOfWeek == nowTime.DayOfWeek).Count();
                count.MonthPublish = articleList.Where(a => a.CreateTime.Month == nowTime.Month).Count();
         
                return count;
            }
        }

        /// <summary>
        /// 统计文章趋势
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<ArticleTrendStatistics>> GetArticleTrend(int? uid = null)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //获取所有数据
                var articleList = await dbContext.Articles.OrderBy(a => a.CreateTime).ToListAsync();
                if (uid.HasValue)
                {
                    articleList = articleList.Where(x => x.UserId == uid.Value).ToList();
                }

                var trends = articleList.GroupBy(a => a.CreateTime.Date).Select(ag => new ArticleTrendStatistics
                {
                    Date = ag.Key,
                    ArticleCount = ag.Count(),
                });
                return trends.ToList();
            }
        }
    }
}
