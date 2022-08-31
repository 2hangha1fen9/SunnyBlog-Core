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
    public class ViewController : ControllerBase
    {
        private readonly IViewApp viewApp;

        public ViewController(IViewApp viewApp)
        {
            this.viewApp = viewApp;
        }

        /// <summary>
        /// 增加文章访问量
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        [RBAC(IsPublic = 1)]
        [HttpGet]
        public async Task<IActionResult> Add(int aid)
        {
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                var address = this.Request.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
                if (string.IsNullOrWhiteSpace(userId))
                {
                    await viewApp.AddArticleViewCount(aid,ip: address);
                }
                else
                {
                    await viewApp.AddArticleViewCount(aid,Convert.ToInt32(userId),address);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 获取用户文章访问记录
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<List<UserViewHistory>>> UserHistory()
        {
            var result = new Response<List<UserViewHistory>>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await viewApp.GetUserView(Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取所有文章访问记录
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<List<UserViewHistory>>> AllHistory()
        {
            var result = new Response<List<UserViewHistory>>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await viewApp.GetUserView(Convert.ToInt32(userId));
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
