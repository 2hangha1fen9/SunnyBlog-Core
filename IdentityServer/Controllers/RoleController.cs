using IdentityService.App.Interface;
using IdentityService.Request;
using IdentityService.Response;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleApp roleApp;

        public RoleController(IRoleApp roleApp)
        {
            this.roleApp = roleApp;
        }

        /// <summary>
        /// 列出角色信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<PResponse<RoleView>> List([FromQuery] int? pageIndex = 1, [FromQuery] int? pageSize = 10, [FromBody] SearchCondition[]? condition = null)
        {
            var result = new PResponse<RoleView>();
            try
            {
                var roles = await roleApp.ListRole(condition);
                result.Pagination(pageIndex.Value, pageSize.Value, roles);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 根据ID查询角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<RoleView>> Get(int id)
        {
            var result = new Response<RoleView>();
            try
            {
                result.Result = await roleApp.GetRoleById(id);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
        
        /// <summary>
        /// 根据用户ID查询角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<PResponse<RoleView>> GetByUser(int id, [FromQuery] int? pageIndex = 1, [FromQuery] int? pageSize = 10)
        {
            var result = new PResponse<RoleView>();
            try
            {
                var roles = await roleApp.GetRoleByUserId(id);
                result.Pagination(pageIndex.Value, pageSize.Value, roles);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
        
        /// <summary>
        /// 根据权限ID查询角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<PResponse<RoleView>> GetByPermission(int id, [FromQuery] int? pageIndex = 1, [FromQuery] int? pageSize = 10)
        {
            var result = new PResponse<RoleView>();
            try
            {
                var roles = await roleApp.GetRoleByPermissionId(id);
                result.Pagination(pageIndex.Value, pageSize.Value, roles);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
    
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPost]
        public async Task<Response<string>> Add(AddRoleReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await roleApp.AddRole(request);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
    
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        public async Task<Response<string>> Del(List<DelRoleReq> request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await roleApp.DelRole(request);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        public async Task<Response<string>> Modify(ModifyRoleReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await roleApp.ChangeRole(request);
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
