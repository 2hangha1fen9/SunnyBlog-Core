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
    public class ManagerController : ControllerBase
    {
        private readonly IArticleApp articleApp;
        private readonly IArticleTagApp tagApp;

        public ManagerController(IArticleApp articleApp, IArticleTagApp tagApp)
        {
            this.articleApp = articleApp;
            this.tagApp = tagApp;
        }

        /// <summary>
        /// 删除文章(管理)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*article*" } })]
        public async Task<Response<string>> RemoveArticle(List<DelArticleReq> request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await articleApp.RemoveArticle(request);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 编辑文章(管理)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*article*" } })]
        public async Task<Response<string>> EditorArticle(EditorArticleReq request)
        {
            var result = new Response<string>();
            try
            {
                var article = await articleApp.EditorArticle(request,null);
                result.Result = article;
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除标签(管理)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*article*","*tag*" } })]
        public async Task<Response<string>> DeleteTag(List<DelTagReq> request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await tagApp.DeletelTag(request);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 列出所有标签(管理)
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<List<TagView>>> ListTag()
        {
            var result = new Response<List<TagView>>();
            try
            {
                var tags = await tagApp.GetAllTags();
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
        /// 获取某个用户的标签(管理)
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<List<TagView>>> ListUserTag(int uid)
        {
            var result = new Response<List<TagView>>();
            try
            {
                var tags = await tagApp.GetUserTags(uid);
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
        /// 更新标签(管理)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*article*", "*tag*" } })]
        public async Task<Response<string>> UpdateTag(UpdateTagReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await tagApp.UpdateTag(request);
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
