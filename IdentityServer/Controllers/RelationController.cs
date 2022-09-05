using IdentityService.App.Interface;
using IdentityService.Request;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [RBAC]
    public class RelationController : ControllerBase
    {
        private readonly IRelationApp relationApp;

        public RelationController(IRelationApp relationApp)
        {
            this.relationApp = relationApp;
        }

        /// <summary>
        /// 角色权限绑定
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<string>> RolePermissionBind(RolePermissionBindReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await relationApp.RolePermissionBind(request);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 用户角色绑定
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<string>> UserRoleBind(UserRoleBindReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await relationApp.UserRoleBind(request);
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
