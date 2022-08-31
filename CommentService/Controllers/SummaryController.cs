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
    public class SummaryController : ControllerBase
    {
        private readonly ICountApp countApp;

        public SummaryController(ICountApp countApp)
        {
            this.countApp = countApp;
        }



        /// <summary>
        /// 获取文章访问量评论数点赞量
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        [RBAC(IsPublic = 1)]
        [HttpGet]
        public async Task<Response<ArticleCountView>> ArticleCount(int aid)
        {
            var result = new Response<ArticleCountView>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    result.Result = await countApp.GetArticleCount(aid);
                }
                else
                {
                    result.Result = await countApp.GetArticleCount(aid, Convert.ToInt32(userId));
                }
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
