using IdentityService.Response;

namespace IdentityService.App.Interface
{
    public interface IStatisticsApp
    {
        Task<PermissionCountStatistics> GetPermissionCount();
    }
}
