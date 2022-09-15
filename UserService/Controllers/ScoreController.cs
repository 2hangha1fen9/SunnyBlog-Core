using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.App.Interface;
using UserService.Domain;
using UserService.Request;

namespace UserService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        private readonly IScoreApp scoreApp;
        public ScoreController(IScoreApp scoreApp)
        {
            this.scoreApp = scoreApp;
        }

        /// <summary>
        /// 用户签到
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<string>> CheckIn()
        {
            var result = new Response<string>();
            try
            {
                var userId = Convert.ToInt32(HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value);
                if (await scoreApp.CanIncrease(userId,"签到"))
                {
                    result.Result = await scoreApp.Increase(userId, "签到");
                }
                else
                {
                    result.Result = $"今日已签到";
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
        /// 列出所有积分单位
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<List<ScoreUnit>>> ListUnit()
        {
            var result = new Response<List<ScoreUnit>>();
            try
            {
                result.Result = await scoreApp.ListUnit();
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
        /// 设置积分单位
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        public async Task<Response<string>> SetUnit(string key,decimal value)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await scoreApp.SetUnit(key,value);
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
