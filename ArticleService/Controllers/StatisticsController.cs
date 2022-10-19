using ArticleService.App.Interface;
using ArticleService.Response;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArticleService.Controllers
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
        /// 统计文章数
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<ArticleCountStatistics>> Count()
        {
            var result = new Response<ArticleCountStatistics>();
            try
            {             
                result.Result = await statisticsApp.GetArticleCount();
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 统计用户的文章数
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<ArticleCountStatistics>> UserCount()
        {
            var result = new Response<ArticleCountStatistics>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await statisticsApp.GetArticleCount(Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 统计文章趋势
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<List<ArticleTrendStatistics>>> Trend()
        {
            var result = new Response<List<ArticleTrendStatistics>>();
            try
            {
                result.Result = await statisticsApp.GetArticleTrend();
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 统计用户文章趋势
        /// </summary>
        /// <returns></returns>
        [RBAC(IsPublic = 1)]
        [HttpGet]
        public async Task<Response<List<ArticleTrendStatistics>>> UserTrend(int uid)
        {
            var result = new Response<List<ArticleTrendStatistics>>();
            try
            {
                result.Result = await statisticsApp.GetArticleTrend(uid);
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
