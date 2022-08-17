﻿using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using UserService.Rpc.Protos;

namespace UserService.Rpc.Service
{
    /// <summary>
    /// gRPC服务类
    /// </summary>
    public class GUserService:gUser.gUserBase
    {
        /// <summary>
        /// 依赖使用
        /// </summary>
        private readonly IDbContextFactory<UserDBContext> contextFactory;
        public GUserService(IDbContextFactory<UserDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async override Task<UserResponse> GetUserID(UserRequest request, ServerCallContext context)
        {
            using (var c= contextFactory.CreateDbContext())
            {
                //查询用户名密码，将结果存入gRPC载荷对象
                var username = request.Username;
                var password = request.Password;
                var user = await c.Users.Where(u => u.Username == username && u.Password == password).FirstOrDefaultAsync();
                var userResponse = new UserResponse()
                {
                    Id = user.Id,
                };
                return userResponse;
            }
        }
    }
}
