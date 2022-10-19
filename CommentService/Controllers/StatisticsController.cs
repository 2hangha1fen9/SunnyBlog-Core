
using CommentService.App.Interface;
using CommentService.Response;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.Controllers
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
        /// 统计评论数
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<CommentCountStatistics>> Count()
        {
            var result = new Response<CommentCountStatistics>();
            try
            {
                result.Result = await statisticsApp.GetCommentCount();
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 统计文章数
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<CommentCountStatistics>> UserCount()
        {
            var result = new Response<CommentCountStatistics>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await statisticsApp.GetCommentCount(Convert.ToInt32(userId));
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
