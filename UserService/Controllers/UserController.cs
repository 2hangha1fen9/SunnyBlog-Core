﻿using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.App.Interface;
using UserService.Response;

namespace UserService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
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
        [HttpGet][RedisCache(expiration:1000)]
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
        /// 根据ID查询用户
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>用户信息</returns>
        [HttpGet][RBAC]
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
    }
}
