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
        [HttpPost]
        public async Task<Response<string>> Article(int aid, int? status = 1)
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

        /// <summary>
        /// 获取点赞消息
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<List<LikeView>>> Message()
        {
            var result = new Response<List<LikeView>>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await likeApp.GetUserLikeMessage(Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除点赞消息
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        public async Task<Response<string>> DeleteMessage(int lid, bool? isAll = false)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await likeApp.DeleteLikeMessage(Convert.ToInt32(userId),lid,isAll.Value);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }


        /// <summary>
        /// 获取用户的点赞/收藏记录
        /// </summary>
        /// <returns></returns>
        [RBAC(IsPublic = 1)]
        [HttpGet]
        public async Task<Response<List<LikeView>>> List(int uid, bool isLike)
        {
            var result = new Response<List<LikeView>>();
            try
            {
                result.Result = await likeApp.GetUserLike(uid, isLike);
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
