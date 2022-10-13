using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UserService.App.Interface;
using UserService.Response;

namespace UserService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IFollowApp followApp;

        public FollowController(IFollowApp followApp)
        {
            this.followApp = followApp;
        }

        /// <summary>
        /// 关注某用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sbId"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPost]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*follow*"} })]
        public async Task<Response<string>> Sb([FromQuery]int id)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await followApp.FollowSb(Convert.ToInt32(userId), id);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
        
        /// <summary>
        /// 查看用户的关注列表/粉丝列表
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(RedisCache))]
        public async Task<Response<List<FollowView>>> List(int id,bool? fans = false, string? condidtion = null)
        {
            var result = new Response<List<FollowView>>();
            try
            {
                List<SearchCondition> con = new List<SearchCondition>();
                try { con = JsonConvert.DeserializeObject<List<SearchCondition>>(condidtion); }
                catch (Exception) { }
                result.Result = await followApp.FollowList(con, id,fans.Value);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取用户关注信息
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="isAll"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<List<FollowView>>> Message()
        {
            var result = new Response<List<FollowView>>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await followApp.GetFollowMessage(Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除用户关注信息
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="isAll"></param>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        public async Task<Response<string>> DeleteMessage(int fid,bool? isAll = false)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await followApp.DeleteFollowMessage(Convert.ToInt32(userId), fid,isAll.Value);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
       
        /// <summary>
        /// 获取关注状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        [TypeFilter(typeof(RedisCache))]
        public async Task<Response<bool>> Status(int id)
        {
            var result = new Response<bool>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await followApp.FollowStatus(Convert.ToInt32(userId), id);
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
