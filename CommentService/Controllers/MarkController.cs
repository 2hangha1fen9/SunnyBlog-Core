using CommentService.App.Interface;
using CommentService.Request;
using CommentService.Response;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MarkController : ControllerBase
    {
        private readonly ILikeApp likeApp;

        public MarkController(ILikeApp likeApp)
        {
            this.likeApp = likeApp;
        }

        /// <summary>
        /// 点赞/收藏文章
        /// </summary>
        /// <param name="aid">文章ID</param>
        /// <param name="status">1点赞2收藏</param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<string>> Article(int aid,int? status = 1)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await likeApp.LikeArticle(aid, Convert.ToInt32(userId),status.Value);
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
