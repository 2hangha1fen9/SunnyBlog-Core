using ArticleService.App.Interface;
using ArticleService.Domain;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArticleService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ISettingApp settingApp;

        public SettingController(ISettingApp settingApp)
        {
            this.settingApp = settingApp;
        }


        /// <summary>
        /// 获取所有设置项
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<List<GlobalSetting>>> List()
        {
            var result = new Response<List<GlobalSetting>>();
            try
            {
                result.Result = await settingApp.ListSetting();
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
                throw;
            }
            return result;
        }
        
        /// <summary>
        /// 更新值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        public async Task<Response<string>> Update(string key,int value)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await settingApp.SetValue(key, value);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
                throw;
            }
            return result;
        }
    }
}
