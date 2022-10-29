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
using static IdentityService.Rpc.Protos.gUser;

namespace Service.IdentityService.App
{
    public class PermissionApp : IPermissionApp
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly IDbContextFactory<AuthDBContext> contextFactory;
        private readonly gUserClient userRpc;

        public PermissionApp(IDbContextFactory<AuthDBContext> contextFactory, gUserClient userRpc)
        {
            this.contextFactory = contextFactory;
            this.userRpc = userRpc;
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
                if (await dbContext.Permissions.FirstOrDefaultAsync(p => p.Service == request.Service && p.Controller == request.Controller && p.Action == request.Action) != null)
                {
                    throw new Exception("权限已存在");
                }

                var permission = new Permission()
                {
                    Service = request.Service,
                    Controller = request.Controller,
                    Action = request.Action,
                    Description = request.Description,
                    Status = request.Status.Value,
                    IsPublic = request.IsPublic,
                };
                await dbContext.Permissions.AddAsync(permission);
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
                    dbContext.Entry(permission).State = EntityState.Modified;
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
            //远程调用
            var user = await userRpc.GetUserByPasswordAsync(new UserRequest()
            {
                Username = username,
                Password = password,
            });
            //查询用户权限
            using (var context = contextFactory.CreateDbContext())
            {
                if(user != null)
                {
                    var permissions = await context.Set<Permission>().FromSqlRaw($"select * from Permission as p where p.id in(select permissionId from RolePermissionRelation as rp where rp.roleId in(select roleId from UserRoleRelation as ur,Role r where ur.roleId = r.id and ur.userId = {user.Id} and exists(select * from User u where id = ur.userId and u.status = 1) and r.status = 1)) and status = 1")
                              .Select(r => new
                              {
                                  r.Controller,
                                  r.Action
                              }).ToArrayAsync();
                    return new object[] { user, permissions };
                }
               
                return new object[] { user, null};
            }
        }

        /// <summary>
        /// 根据账户名获取用户权限
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<object[]> GetPermission(string username)
        {
            //远程调用
            var user = await userRpc.GetUserByUsernameAsync(new UserNameRequest()
            {
                Username = username,
            });

            //查询用户权限
            using (var context = contextFactory.CreateDbContext())
            {
                if(user != null)
                {
                    var permissions = await context.Set<Permission>().FromSqlRaw($"select * from Permission as p where p.id in(select permissionId from RolePermissionRelation as rp where rp.roleId in(select roleId from UserRoleRelation as ur,Role r where ur.roleId = r.id and ur.userId = {user.Id} and exists(select * from User u where id = ur.userId and u.status = 1) and r.status = 1)) and status = 1")
                            .Select(r => new
                            {
                                r.Controller,
                                r.Action
                            }).ToArrayAsync();
                    return new object[] { user, permissions };
                }
                return new object[] { user, null };
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
        public async Task<PageList<PermissionView>> GetPermissionsByRoleId(int id,int pageIndex, int pageSize)
        {
            //查询用户权限
            using (var context = contextFactory.CreateDbContext())
            {
                /*var permissions = await context.Set<Permission>().FromSqlRaw($"select * from Permission as p where p.id in(select permissionId from RolePermissionRelation as rp where rp.roleId in(select roleId from UserRoleRelation as ur,Role r where ur.roleId = {id} and r.status = 1)) and status = 1")
                           .ToListAsync();*/
                var permissions = context.RolePermissionRelations.Where(rpr => rpr.RoleId == id).Select(p => new Permission
                {
                    Id = p.PermissionId,
                    Service = p.Permission.Service,
                    Controller = p.Permission.Controller,
                    Action = p.Permission.Action,
                    UpdateTime = p.Permission.UpdateTime,
                    CreateTime = p.Permission.CreateTime,
                    Description = p.Permission.Description,
                    Status = p.Permission.Status,
                    IsPublic = p.Permission.IsPublic
                });
                //对结果分页
                var permissionPage = new PageList<PermissionView>();
                permissions = permissionPage.Pagination(pageIndex, pageSize, permissions);
                permissionPage.Page = (await permissions.ToListAsync()).MapToList<PermissionView>();
                return permissionPage;
            }
        }

        /// <summary>
        /// 根据用户Id获取权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PageList<PermissionView>> GetPermissionsByUserId(int id, int pageIndex, int pageSize)
        {
            //查询用户权限
            using (var context = contextFactory.CreateDbContext())
            {
                var permissions = context.Set<Permission>().FromSqlRaw($"select * from Permission as p where p.id in(select permissionId from RolePermissionRelation as rp where rp.roleId in(select roleId from UserRoleRelation as ur,Role r where ur.roleId = r.id and ur.userId = {id} and exists(select * from User u where id = ur.userId and u.status = 1) and r.status = 1)) and status = 1");
                //对结果分页
                var permissionPage = new PageList<PermissionView>();
                permissions = permissionPage.Pagination(pageIndex, pageSize, permissions);
                permissionPage.Page = (await permissions.ToListAsync()).MapToList<PermissionView>();
                return permissionPage;
            }
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="condidtion"></param>
        /// <returns></returns>
        public async Task<PageList<PermissionView>> ListPermission(List<SearchCondition>? condidtion, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var permissions = dbContext.Permissions.OrderBy(p => p.Service).AsEnumerable();
                if (condidtion?.Count > 0)
                {
                    foreach (var con in condidtion)
                    {
                        permissions = "Id".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? permissions.Where(p => p.Id == Convert.ToInt32(con.Value)) : permissions; 
                        permissions = "Service".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? permissions.Where(p => p.Service.Contains(con.Value, StringComparison.OrdinalIgnoreCase)) : permissions; 
                        permissions = "Controller".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? permissions.Where(p => p.Controller.Contains(con.Value, StringComparison.OrdinalIgnoreCase)) : permissions; 
                        permissions = "Action".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? permissions.Where(p => p.Action.Contains(con.Value, StringComparison.OrdinalIgnoreCase)) : permissions; 
                        permissions = "Description".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? permissions.Where(p => p.Description.Contains(con.Value, StringComparison.OrdinalIgnoreCase)) : permissions;
                        permissions = "Status".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? permissions.Where(p => p.Status == Convert.ToInt32(con.Value)) : permissions;
                        permissions = "IsPublic".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? permissions.Where(p => p.IsPublic == Convert.ToInt32(con.Value)) : permissions;
                        //排序
                        if (con.Sort != 0)
                        {
                            if ("CreateTime".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                            {
                                if (con.Sort == -1)
                                {
                                    permissions = permissions.OrderByDescending(a => a.CreateTime);
                                }
                                else
                                {
                                    permissions = permissions.OrderBy(a => a.CreateTime);
                                }
                            }
                            if ("UpdateTime".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                            {
                                if (con.Sort == -1)
                                {
                                    permissions = permissions.OrderByDescending(a => a.UpdateTime);
                                }
                                else
                                {
                                    permissions = permissions.OrderBy(a => a.UpdateTime);
                                }
                            }
                        }
                    }
                }
                //对结果分页
                var permissionPage = new PageList<PermissionView>();
                permissions = permissionPage.Pagination(pageIndex, pageSize, permissions.AsQueryable());
                permissionPage.Page = permissions.ToList().MapToList<PermissionView>();
                return permissionPage;
            }
        }
    }
}
