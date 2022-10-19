using ArticleService.Response;

namespace ArticleService.App.Interface
{
    public interface IStatisticsApp
    {
        Task<ArticleCountStatistics> GetArticleCount(int? uid = null);
        Task<List<ArticleTrendStatistics>> GetArticleTrend(int? uid = null);
    }
}
