using IdentityService.App.Interface;
using IdentityService.Request;
using IdentityService.Response;
using Infrastructure;
using Infrastructure.Auth;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace IdentityService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [RBAC]
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
        [HttpGet]
        public async Task<Response<PageList<RoleView>>> List(int? pageIndex = 1,int? pageSize = 10, string? condition = null)
        {
            var result = new Response<PageList<RoleView>>();
            try
            {
                List<SearchCondition> con = SequenceConverter.ConvertToCondition(condition);
                var roles = await roleApp.ListRole(con, pageIndex.Value,pageSize.Value);
                result.Result = roles;
                return result;
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
        [HttpGet]
        public async Task<Response<PageList<RoleView>>> GetByUser(int id,int? pageIndex = 1,int? pageSize = 10)
        {
            var result = new Response<PageList<RoleView>>();
            try
            {
                var roles = await roleApp.GetRoleByUserId(id,pageIndex.Value,pageSize.Value);
                result.Result = roles;
                return result;
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
        [HttpGet]
        public async Task<Response<PageList<RoleView>>> GetByPermission(int id,int? pageIndex = 1,int? pageSize = 10)
        {
            var result = new Response<PageList<RoleView>>();
            try
            {
                var roles = await roleApp.GetRoleByPermissionId(id, pageIndex.Value, pageSize.Value);
                result.Result = roles;
                return result;
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
