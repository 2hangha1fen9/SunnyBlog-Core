using IdentityService.App.Interface;
using IdentityService.Domain;
using IdentityService.Request;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Service.IdentityService.App.Interface;

namespace IdentityService.App
{
    public class RelationApp : IRelationApp
    {
        private readonly IDbContextFactory<AuthDBContext> contextFactory;

        public RelationApp(IDbContextFactory<AuthDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// 角色权限绑定
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> RolePermissionBind(RolePermissionBindReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                try
                {
                    //获取角色所有权限
                    var rolePermission = await dbContext.RolePermissionRelations.Where(p => p.RoleId == request.RoleId).ToListAsync();
                    //获取所有权限
                    var permissions = await dbContext.Permissions.ToListAsync();
                    //先清空当前角色的权限
                    foreach (var item in rolePermission)
                    {
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                    //添加最新的权限
                    foreach (var pid in request.PermissionIds)
                    {
                        if (permissions.FirstOrDefault(p => p.Id  == pid) != null)
                        {
                            dbContext.Entry(new RolePermissionRelation()
                            {
                                RoleId = request.RoleId,
                                PermissionId = pid,
                            }).State = EntityState.Added;
                        }
                    }
                    await dbContext.SaveChangesAsync();
                    return "绑定成功";
                }
                catch (Exception)
                {
                    throw new Exception("部分绑定失败");
                }
            }
        }

        /// <summary>
        /// 用户角色绑定
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> UserRoleBind(UserRoleBindReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                try
                {
                    //获取用户所有角色
                    var userRole = await dbContext.UserRoleRelations.Where(u => u.UserId == request.UserId).ToListAsync();
                    //获取所有角色
                    var role = await dbContext.Roles.ToListAsync();
                    //清空用户的所有角色
                    foreach (var item in userRole)
                    {
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                    //添加最新的角色
                    foreach (var rid in request.RoleIds)
                    {
                        if (role.FirstOrDefault(r => r.Id == rid) != null)
                        {
                            dbContext.Entry(new UserRoleRelation()
                            {
                                UserId = request.UserId,
                                RoleId = rid,
                            }).State = EntityState.Added;
                        }
                    }
                    await dbContext.SaveChangesAsync();
                    return "绑定成功";
                }
                catch (Exception)
                {
                    throw new Exception("部分绑定失败");
                }
            }
        }
    }
}
