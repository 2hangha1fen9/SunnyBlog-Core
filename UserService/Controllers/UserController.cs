using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        private readonly IAdminApp adminApp;

        public UserController(IUserApp userApp,IAdminApp adminApp)
        {
            this.userApp = userApp;
            this.adminApp = adminApp;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns>用户列表</returns>
        [HttpGet]
        [RBAC(IsPublic = 1)]
        public async Task<PResponse<UserView>> List([FromQuery] int? pageIndex =1 , [FromQuery] int? pageSize = 10, [FromBody] SearchCondition[]? condidtion = null)
        {
            var result = new PResponse<UserView>();
            try
            {
                var users = await userApp.GetUsers(condidtion);
                //分页
                result.Pagination(pageIndex.Value, pageSize.Value, users);
            }
            catch (Exception ex)
            {
                result.Status = 500;
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
        public async Task<Response<UserDetailView>> LoginInfo()
        {
            var result = new Response<UserDetailView>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await adminApp.GetUserDetailById(Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RBAC(IsPublic = 1)]
        public async Task<Response<string>> Register(UserRegisterReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await userApp.UserRegister(request);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 找回密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RBAC(IsPublic = 1)]
        public async Task<Response<string>> ForgetPassword(ForgetPasswordReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await userApp.ChangePassword(request);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        public async Task<Response<string>> UpdateInfo(UpdateUserReq request)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await userApp.ChangeUser(request, Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 账号绑定
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        public async Task<Response<string>> BindAccount(BindAccountReq request)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await userApp.BindAccount(request, Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 账号解绑
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        public async Task<Response<string>> UbindAccount(UbindAccountReq request)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await userApp.UbindAccount(request, Convert.ToInt32(userId));
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
