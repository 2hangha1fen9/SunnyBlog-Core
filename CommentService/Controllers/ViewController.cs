using CommentService.App.Interface;
using CommentService.Response;
using Infrastructure;
using Infrastructure.Auth;
using Infrastructure.Utils;
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
        /// 列出用户文章访问记录
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<PageList<UserViewHistory>>> UserHistory(int? pageIndex = 1, int? pageSize = 10, string? condition = null)
        {
            var result = new Response<PageList<UserViewHistory>>();
            try
            {
                List<SearchCondition> con = SequenceConverter.ConvertToCondition(condition);
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await viewApp.GetUserView(Convert.ToInt32(userId), con, pageIndex.Value, pageSize.Value);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 列出所有文章访问记录
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<PageList<UserViewHistory>>> AllHistory(int? pageIndex = 1, int? pageSize = 10, string? condition = null)
        {
            var result = new Response<PageList<UserViewHistory>>();
            try
            {
                List<SearchCondition> con = SequenceConverter.ConvertToCondition(condition);
                result.Result = await viewApp.GetViewList(con, pageIndex.Value, pageSize.Value);
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
