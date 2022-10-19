using IdentityService.App.Interface;
using IdentityService.Response;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.App
{
    public class StatisticsApp: IStatisticsApp
    {
        private readonly IDbContextFactory<AuthDBContext> contextFactory;

        public StatisticsApp(IDbContextFactory<AuthDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// 获取所有权限统计
        /// </summary>
        /// <returns></returns>
        public async Task<PermissionCountStatistics> GetPermissionCount()
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var permisssionList = await dbContext.Permissions.ToListAsync();
                var roleList = await dbContext.Roles.ToListAsync();
                var count = new PermissionCountStatistics();
                count.PermissionCount = permisssionList.Count;
                count.RoleCount = roleList.Count;
                count.EnablePermissionCount = permisssionList.Where(p => p.Status == 1).Count();
                count.DisablePermissionCount = permisssionList.Where(p => p.Status == -1).Count();
                count.EnableRoleCount = roleList.Where(p => p.Status == 1).Count();
                count.DisableRoleCount = roleList.Where(p => p.Status == -1).Count();

                return count;
            }
        }
    }
}
