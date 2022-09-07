using IdentityService.Request;
using IdentityService.Response;
using Infrastructure;
using Infrastructure.Auth;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Service.IdentityService.App.Interface;

namespace IdentityService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [RBAC]
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
        [HttpGet]
        public async Task<Response<PageList<PermissionView>>> List(int? pageIndex = 1,int? pageSize = 10,string? condition = null)
        {
            var result = new Response<PageList<PermissionView>>();
            try
            {
                List<SearchCondition> con = SequenceConverter.ConvertToCondition(condition);
                var permissions = await permissionApp.ListPermission(con, pageIndex.Value,pageSize.Value);
                result.Result = permissions;
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
        /// 根据用户ID列出权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<PageList<PermissionView>>> GetByUser(int id,int? pageIndex = 1, int? pageSize = 10)
        {
            var result = new Response<PageList<PermissionView>>();
            try
            {
                var permissions = await permissionApp.GetPermissionsByUserId(id,pageIndex.Value,pageSize.Value);
                result.Result = permissions;
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
        /// 根据角色ID列出权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<PageList<PermissionView>>> GetByRole(int id,  int? pageIndex = 1, int? pageSize = 10)
        {
            var result = new Response<PageList<PermissionView>>();
            try
            {
                var permissions = await permissionApp.GetPermissionsByRoleId(id,pageIndex.Value,pageSize.Value);
                result.Result = permissions;
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
        /// 添加权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*publicPermission*" } })]
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
        [HttpDelete]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] {"*publicPermission*"}})]
        public async Task<Response<string>> Delete(List<DelPermissionReq> request)
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
        [HttpPut]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*publicPermission*" } })]
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
