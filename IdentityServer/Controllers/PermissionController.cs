using IdentityService.Request;
using IdentityService.Response;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.IdentityService.App.Interface;

namespace IdentityService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionApp permissionApp;

        public PermissionController(IPermissionApp permissionApp)
        {
            this.permissionApp = permissionApp;
        }

        /// <summary>
        /// 列出权限信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<PResponse<PermissionView>> List([FromQuery] int? pageIndex = 1, [FromQuery] int? pageSize = 10, [FromBody] SearchCondition[]? condition = null)
        {
            var result = new PResponse<PermissionView>();
            try
            {
                var permissions = await permissionApp.ListPermission(condition);
                result.Pagination(pageIndex.Value, pageSize.Value, permissions);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
        
        /// <summary>
        /// 根据ID查询权限详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<PermissionView>> Get(int id)
        {
            var result = new Response<PermissionView>();
            try
            {
                result.Result = await permissionApp.GetPermissionById(id);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 根据用户ID查询权限列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<PResponse<PermissionView>> GetByUser(int id,[FromQuery] int? pageIndex = 1, [FromQuery] int? pageSize = 10)
        {
            var result = new PResponse<PermissionView>();
            try
            {
                var permissions = await permissionApp.GetPermissionsByUserId(id);
                result.Pagination(pageIndex.Value, pageSize.Value, permissions);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 根据用户ID查询权限列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<PResponse<PermissionView>> GetByRole(int id, [FromQuery] int? pageIndex = 1, [FromQuery] int? pageSize = 10)
        {
            var result = new PResponse<PermissionView>();
            try
            {
                var permissions = await permissionApp.GetPermissionsByRoleId(id);
                result.Pagination(pageIndex.Value, pageSize.Value, permissions);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPost]
        public async Task<Response<string>> Add(AddPermissionReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await permissionApp.AddPermission(request);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
    
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        public async Task<Response<string>> Del(List<DelPermissionReq> request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await permissionApp.DelPermission(request);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
    
        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        public async Task<Response<string>> Modify(ModifyPermissionReq request)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await permissionApp.ChangePermission(request);
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
