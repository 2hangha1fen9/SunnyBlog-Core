using ArticleService.App.Interface;
using ArticleService.Request;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArticleService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SiteConfigController : ControllerBase
    {
        private readonly ISiteConfigApp siteConfigApp;

        public SiteConfigController(ISiteConfigApp siteConfigApp)
        {
            this.siteConfigApp = siteConfigApp;
        }

        /// <summary>
        /// 获取网站配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [RBAC(IsPublic = 1)]
        [HttpGet]
        public async Task<Response<string>> GetValue(string key)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await siteConfigApp.GetConfig(key);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 设置网站配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        public async Task<Response<string>> SetValue([FromBody] SiteConfigReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await siteConfigApp.SetConfig(request.Key,request.Value);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除网站配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        public async Task<Response<string>> DelValue(string key)
        {
            var result = new Response<string>();
            try
            { 
                result.Result = await siteConfigApp.DelConfig(key);
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
