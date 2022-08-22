using Grpc.Net.Client;
using IdentityService;
using IdentityService.Domain;
using IdentityService.Request;
using IdentityService.Response;
using IdentityService.Rpc.Protos;
using Infrastructure;
using Infrastructure.Consul;
using Microsoft.EntityFrameworkCore;
using Service.IdentityService.App.Interface;



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
        /// 添加权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> AddPermission(AddPermissionReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var permission = new Permission()
                {
                    Service = request.Service,
                    Controller = request.Controller,
                    Action = request.Action,
                    Description = request.Description,
                    Status = request.Status.Value,
                    IsPublic = request.IsPublic,
                };
                dbContext.Permissions.Add(permission);
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("添加失败");
                }
                else
                {
                    return "添加成功";
                }
            }
        }

        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> ChangePermission(ModifyPermissionReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var permission = await dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == request.Id);
                if (permission != null)
                {
                    permission.Service = request.Service ?? permission.Service;
                    permission.Controller = request.Controller ?? permission.Controller;
                    permission.Action = request.Action ?? permission.Action;
                    permission.Description = request.Description ?? permission.Description;
                    permission.Status = request.Status ?? permission.Status;
                    permission.IsPublic = request.IsPublic ?? permission.IsPublic;
                    permission.UpdateTime = DateTime.Now;
                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        return "修改成功";
                    }
                    else
                    {
                        throw new Exception("修改失败");
                    }
                }
                else
                {
                    throw new Exception("权限信息异常");
                }
            }
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        public async Task<string> DelPermission(List<DelPermissionReq> requests)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var permission = requests.MapToList<Permission>();
                dbContext.Permissions.RemoveRange(permission);
                if (await dbContext.SaveChangesAsync() > 0)
                {
                    return "删除成功";
                }
                else
                {
                    throw new Exception("删除失败");
                }
            }
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
                var permissions = await context.Set<Permission>().FromSqlRaw($"select * from Permission as p where p.id in(select permissionId from RolePermissionRelation as rp where rp.roleId in(select roleId from UserRoleRelation as ur,Role r where ur.roleId = r.id and ur.userId = {user.Id} and exists(select * from [User] u where id = ur.userId and u.status = 1) and r.status = 1)) and status = 1")
                            .Select(r => new
                            {
                                r.Controller,
                                r.Action
                            }).ToArrayAsync();
                return new object[] { user.Id, permissions};
            }
        }

        /// <summary>
        /// 根据ID获取权限详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PermissionView> GetPermissionById(int id)
        {
            //查询用户权限
            using (var context = contextFactory.CreateDbContext())
            {
                var permissions = await context.Permissions.FirstOrDefaultAsync(p => p.Id == id);
                return permissions.MapTo<PermissionView>();
            }
        }

        /// <summary>
        /// 根据角色查询权限信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<PermissionView>> GetPermissionsByRoleId(int id)
        {
            //查询用户权限
            using (var context = contextFactory.CreateDbContext())
            {
                var permissions = await context.Set<Permission>().FromSqlRaw($"select * from Permission as p where p.id in(select permissionId from RolePermissionRelation as rp where rp.roleId in(select roleId from UserRoleRelation as ur,Role r where ur.roleId = {id} and r.status = 1)) and status = 1")
                           .ToListAsync();
                return permissions.MapToList<PermissionView>();
            }
        }

        /// <summary>
        /// 根据用户Id获取权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<PermissionView>> GetPermissionsByUserId(int id)
        {
            //查询用户权限
            using (var context = contextFactory.CreateDbContext())
            {
                var permissions = await context.Set<Permission>().FromSqlRaw($"select * from Permission as p where p.id in(select permissionId from RolePermissionRelation as rp where rp.roleId in(select roleId from UserRoleRelation as ur,Role r where ur.roleId = r.id and ur.userId = {id} and exists(select * from [User] u where id = ur.userId and u.status = 1) and r.status = 1)) and status = 1")
                            .ToListAsync();
                return permissions.MapToList<PermissionView>();
            }
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="condidtion"></param>
        /// <returns></returns>
        public async Task<List<PermissionView>> ListPermission(SearchCondition[] condidtion)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var permissions = dbContext.Permissions.AsQueryable();
                if (condidtion.Length > 0)
                {
                    foreach (var con in condidtion)
                    {
                        permissions = "Id".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? permissions.Where(p => p.Id == Convert.ToInt32(con.Value)) : permissions; 
                        permissions = "Service".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? permissions.Where(p => p.Service.Contains(con.Value)) : permissions; 
                        permissions = "Controller".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? permissions.Where(p => p.Controller.Contains(con.Value)) : permissions; 
                        permissions = "Action".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? permissions.Where(p => p.Action.Contains(con.Value)) : permissions; 
                        permissions = "Description".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? permissions.Where(p => p.Description.Contains(con.Value)) : permissions;
                        permissions = "Status".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? permissions.Where(p => p.Status == Convert.ToInt32(con.Value)) : permissions;
                        permissions = "IsPublic".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? permissions.Where(p => p.IsPublic == Convert.ToInt32(con.Value)) : permissions;
                    }
                }
                var result = await permissions.ToListAsync();
                var permissionView = result.MapToList<PermissionView>();
                return permissionView;
            }
        }
    }
}
