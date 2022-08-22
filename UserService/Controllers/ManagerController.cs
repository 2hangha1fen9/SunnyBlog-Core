using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UserService.App.Interface;
using UserService.Request;
using UserService.Request.Admin;
using UserService.Request.Request.Admin;
using UserService.Response;

namespace UserService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [RBAC]
    public class ManagerController : ControllerBase
    {
        private readonly IAdminApp userApp;
        public ManagerController(IAdminApp userApp)
        {
            this.userApp = userApp;
        }

        /// <summary>
        /// 获取用户详情列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<PResponse<UserDetailView>> ListUser([FromQuery]int? pageIndex = 1,[FromQuery]int? pageSize = 10,[FromBody]SearchCondition[]? condition = null)
        {
            var result = new PResponse<UserDetailView>();
            try
            {
                var list = await userApp.GetUserDetails(condition);
                //分页
                result.Pagination(pageIndex.Value, pageSize.Value, list);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 根据ID查询用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<UserDetailView>> GetUser(int id)
        {
            var result = new Response<UserDetailView>();
            try
            {
                result.Result = await userApp.GetUserDetailById(id);
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
        [HttpPut]
        public async Task<Response<string>> ModifyUser(ModifyUserReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await userApp.ChangeUserDetail(request);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<string>> AddUser(AddUserReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await userApp.AddUser(request);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<Response<string>> DelUser(List<DelUserReq> requests)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await userApp.DelUser(requests);
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
