using ArticleService.App.Interface;
using ArticleService.Request;
using ArticleService.Response;
using Infrastructure;
using Infrastructure.Auth;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ArticleService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ArticleController:ControllerBase
    {
        private readonly IArticleApp articleApp;

        public ArticleController(IArticleApp articleApp)
        {
            this.articleApp = articleApp;
        }

        /// <summary>
        /// 列出文章列表
        /// </summary>
        /// <param name="condidtion"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [RBAC(IsPublic = 1)]
        [HttpGet]
        [TypeFilter(typeof(RedisCache))]
        public async Task<Response<PageList<ArticleListView>>> List(string? condition = null, int? pageIndex = 1,int? pageSize = 10)
        {
            var result = new Response<PageList<ArticleListView>>();
            try
            {
                List<SearchCondition> con = SequenceConverter.ConvertToCondition(condition);
                var article = await articleApp.GetArticleList(con,a => a.Status == 1, pageIndex.Value, pageSize.Value);
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
        /// 列出所有文章列表
        /// </summary>
        /// <param name="condidtion"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        [TypeFilter(typeof(RedisCache))]
        public async Task<Response<PageList<ArticleListView>>> All(string? condition = null, int? pageIndex = 1,  int? pageSize = 10)
        {
            var result = new Response<PageList<ArticleListView>>();
            try
            {
                List<SearchCondition> con = SequenceConverter.ConvertToCondition(condition);
                var article = await articleApp.GetArticleList(con,null,pageIndex.Value, pageSize.Value);
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
        /// 列出用户发布的文章列表
        /// </summary>
        /// <returns></returns>
        [RBAC(IsPublic = 1)]
        [HttpGet]
        [TypeFilter(typeof(RedisCache))]
        public async Task<Response<PageList<ArticleListView>>> User(int uid,string? condition = null, int? pageIndex = 1, int? pageSize = 10)
        {
            var result = new Response<PageList<ArticleListView>>();
            try
            {
                List<SearchCondition> con = SequenceConverter.ConvertToCondition(condition);
                var article = await articleApp.GetArticleList(con, a => a.UserId == uid && a.Status == 1, pageIndex.Value, pageSize.Value);
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
        /// 列出我的文章列表
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<PageList<ArticleListView>>> My(string? condition = null, int? pageIndex = 1, int? pageSize = 10)
        {
            var result = new Response<PageList<ArticleListView>>();
            try
            {
                List<SearchCondition> con = SequenceConverter.ConvertToCondition(condition);
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                var article = await articleApp.GetArticleList(con, a => a.UserId == Convert.ToInt32(userId), pageIndex.Value, pageSize.Value);
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
        /// 浏览文章正文
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RBAC(IsPublic = 1)]
        [HttpGet]
        [TypeFilter(typeof(RedisCache))]
        public async Task<Response<ArticleView>> Detail(int id)
        {
            var result = new Response<ArticleView>();
            try
            {
                var article = await articleApp.GetArticle(id);
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
        /// 浏览文章正文,无视文章状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        [TypeFilter(typeof(RedisCache))]
        public async Task<Response<ArticleView>> DetailIgnoreStatus(int id)
        {
            var result = new Response<ArticleView>();
            try
            {
                var article = await articleApp.GetArticle(id,true);
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
        /// 编辑文章
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*article*" } })] 
        public async Task<Response<string>> Editor(EditorArticleReq request)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                var article = await articleApp.EditorArticle(request, Convert.ToInt32(userId));
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
        /// 发布文章
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPost]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*article*" } })]
        public async Task<Response<string>> Publish(PublishArticleReq request)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                var article = await articleApp.PublishArticle(request, Convert.ToInt32(userId));
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
        /// 删除文章
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*article*" } })]
        public async Task<Response<string>> Remove(List<DelArticleReq> request)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await articleApp.RemoveArticle(request, Convert.ToInt32(userId));
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
