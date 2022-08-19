using Grpc.Net.Client;
using IdentityService.Rpc.Protos;
using Infrastructure;
using Infrastructure.Consul;
using Microsoft.EntityFrameworkCore;
using Service.IdentityService.App.Interface;
using Service.IdentityService.Domain;


namespace Service.IdentityService.App
{
    public class PermissionApp : IPermissionApp
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly IDbContextFactory<AuthDBContext> contextFactory;
        //获取配置
        private readonly IConfiguration config;

        public PermissionApp(IDbContextFactory<AuthDBContext> contextFactory,IConfiguration config)
        {
            this.contextFactory = contextFactory;
            this.config = config;
        }

        /// <summary>
        /// 根据用户账号密码获取权限
        /// </summary>
        /// <param name="user"></param>
        /// <returns>第一个元素用户ID,第二元素权限列表</returns>
        public async Task<object[]> GetPermission(string username,string password)
        {
            //调用consul服务发现，获取rpc服务地址
            var url = ServiceUrl.GetServiceUrlByName("UserService",
                        config.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress);
            //创建通讯频道
            using var channel = GrpcChannel.ForAddress(url);
            //创建客户端
            var client = new gUser.gUserClient(channel);
            //远程调用
            var user = await client.GetUserIDAsync(new UserRequest()
            {
                Username = username,
                Password = password,
            });
            //查询用户权限
            using (var context = contextFactory.CreateDbContext())
            {
                var permissions = context.Set<Permission>().FromSqlRaw($"select * from Permission as p where p.id in(select permissionId from RolePermissionRelation as rp where rp.roleId in(select roleId from UserRoleRelation as ur,Role r where ur.roleId = r.id and ur.userId = {user.Id} and r.status = 1)) and status = 1")
                            .Select(r => new
                            {
                                r.Controller,
                                r.Action
                            }).ToArrayAsync();
                return new object[] { user.Id, permissions.Result };
            }
        }
    }
}
