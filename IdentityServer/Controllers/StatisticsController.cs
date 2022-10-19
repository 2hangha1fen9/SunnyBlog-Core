
using IdentityService.App.Interface;
using IdentityService.Response;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsApp statisticsApp;

        public StatisticsController(IStatisticsApp statisticsApp)
        {
            this.statisticsApp = statisticsApp;
        }

        /// <summary>
        /// 获取所有权限统计
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<PermissionCountStatistics>> Count()
        {
            var result = new Response<PermissionCountStatistics>();
            try
            {
                result.Result = await statisticsApp.GetPermissionCount();
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
    }
}
