using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using UserService.App.Interface;
using UserService.Request;
using UserService.Response;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserApp userApp;
        public UserController(IUserApp userApp)
        {
            this.userApp = userApp;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns>用户列表</returns>
        [HttpGet]
        [RedisCache(expiration: 1000)]
        public async Task<Response<List<UserView>>> List()
        {
            var result = new Response<List<UserView>>();
            try
            {
                result.Result = await userApp.GetUsers();
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取用户详情列表
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        [RedisCache(expiration: 1000)]
        public async Task<Response<List<UserDetailView>>> Details()
        {
            var result = new Response<List<UserDetailView>>();
            try
            {
                result.Result = await userApp.GetUserDetails();
                
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 根据ID查询用户
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>用户信息</returns>

        [HttpGet]
        [RedisCache(expiration: 1000)]
        public async Task<Response<UserView>> GetUser(int id)
        {
            var result = new Response<UserView>();
            try
            {
                result.Result = await userApp.GetUserById(id);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<UserView>> GetMyUser()
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
            return await GetUser(Convert.ToInt32(userId));
        }

        /// <summary>
        /// 根据ID查询用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<UserDetailView>> GetUserDetail(int id)
        {
            var result = new Response<UserDetailView>();
            try
            {
                result.Result = await userApp.GetUserDetailById(id);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取当前用户的详情
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<UserDetailView>> GetMyUserDetail()
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
            return await GetUserDetail(Convert.ToInt32(userId));
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<string>> Register(UserRegisterReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await userApp.UserRegister(request);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
    }
}
