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
    public class MetaController : ControllerBase
    {
        private readonly IMetaApp countApp;

        public MetaController(IMetaApp countApp)
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
        public  Response<Meta> GetArticleMeta(int aid)
        {
            var result = new Response<Meta>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    result.Result = countApp.GetMeta(aid);
                }
                else
                {
                    result.Result = countApp.GetMeta(aid, Convert.ToInt32(userId));
                }
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取一批文章访问量评论数点赞量
        /// </summary>
        /// <param name="aids"></param>
        /// <returns></returns>
        [RBAC(IsPublic = 1)]
        [HttpPost]
        public Response<List<Meta>> GetArticleMetaList(int[] aids)
        {
            var result = new Response<List<Meta>>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    result.Result = countApp.GetMetaList(aids);
                }
                else
                {
                    result.Result = countApp.GetMetaList(aids, Convert.ToInt32(userId));
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
