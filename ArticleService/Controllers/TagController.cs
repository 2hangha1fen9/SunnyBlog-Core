using ArticleService.App.Interface;
using ArticleService.Request;
using ArticleService.Response;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArticleService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly IArticleTagApp articleTagApp;

        public TagController(IArticleTagApp articleTagApp)
        {
            this.articleTagApp = articleTagApp;
        }

        /// <summary>
        /// 列出所有公共标签
        /// </summary>
        /// <returns></returns>
        [RBAC(IsPublic = 1)]
        [HttpGet]
        [TypeFilter(typeof(RedisCache))]
        public async Task<Response<List<TagView>>> Public()
        {
            var result = new Response<List<TagView>>();
            try
            {
                var tags = await articleTagApp.GetPublicTags();
                result.Result = tags;
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 列出用户个人标签
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<List<TagView>>> My()
        {
            var result = new Response<List<TagView>>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                var tags = await articleTagApp.GetUserTags(Convert.ToInt32(userId));
                result.Result = tags;
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 列出用户的标签
        /// </summary>
        /// <returns></returns>
        [RBAC(IsPublic = 1)]
        [HttpGet]
        public async Task<Response<List<TagView>>> UserTag(int uid)
        {
            var result = new Response<List<TagView>>();
            try
            {
                var tags = await articleTagApp.GetUserTags(uid);
                result.Result = tags.Where(t => t.UserId == uid).ToList();
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpPost]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*article*", "*tag*" } })]
        public async Task<Response<string>> Create(AddTagReq request)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await articleTagApp.CreateTag(request, Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*article*", "*tag*" } })]
        public async Task<Response<string>> Delete(List<DelTagReq> request)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await articleTagApp.DeletelTag(request, Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 更新标签
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*article*", "*tag*" } })]
        public async Task<Response<string>> Update(UpdateTagReq request)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await articleTagApp.UpdateTag(request, Convert.ToInt32(userId));
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
