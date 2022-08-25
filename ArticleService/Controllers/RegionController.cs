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
    public class RegionController : ControllerBase
    {
        private readonly IArticleRegionApp articleRegionApp;

        public RegionController(IArticleRegionApp articleRegionApp)
        {
            this.articleRegionApp = articleRegionApp;
        }

        /// <summary>
        /// 获取分区视图
        /// </summary>
        /// <returns></returns>
        [RBAC(IsPublic = 1)]
        [HttpGet]
        public async Task<Response<List<RegionView>>> List()
        {
            var result = new Response<List<RegionView>>();
            try
            {
                var regions = await articleRegionApp.GetRegions();
                result.Result = regions;
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 更新分区
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        public async Task<Response<string>> Update(UpdateRegionReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await articleRegionApp.UpdateRegion(request);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除分区
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        public async Task<Response<string>> Delete(List<DelRegionReq> request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await articleRegionApp.DeletelRegion(request);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 创建分区
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPost]
        public async Task<Response<string>> Create(AddRegionReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await articleRegionApp.CreateRegion(request);
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
