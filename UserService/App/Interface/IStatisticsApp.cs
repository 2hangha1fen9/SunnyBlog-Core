using UserService.Response;

namespace UserService.App.Interface
{
    public interface IStatisticsApp
    {
        Task<UserCountStatistics> GetUserCount();
        Task<List<UserTrendStatistics>> GetUserTrend();
    }
}
