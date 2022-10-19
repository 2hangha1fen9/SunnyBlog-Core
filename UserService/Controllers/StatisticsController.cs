
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.App.Interface;
using UserService.Response;

namespace UserService.Controllers
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
        /// 统计用户数
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<UserCountStatistics>> Count()
        {
            var result = new Response<UserCountStatistics>();
            try
            {
                result.Result = await statisticsApp.GetUserCount();
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        ///统计用户趋势
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<List<UserTrendStatistics>>> Trend()
        {
            var result = new Response<List<UserTrendStatistics>>();
            try
            {
                result.Result = await statisticsApp.GetUserTrend();
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
