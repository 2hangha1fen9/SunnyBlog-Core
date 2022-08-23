using IdentityService.App.Interface;
using IdentityService.Domain;
using IdentityService.Request;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

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
        public async Task<string> RolePermissionBind(RolePermissionBindReq request,bool isRemove = false)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                try
                {
                    foreach (var pid in request.PermissionIds)
                    {
                        var rpr = await dbContext.RolePermissionRelations.FirstOrDefaultAsync(rpr => rpr.RoleId == request.RoleId && rpr.PermissionId == pid);
                        if (rpr == null && !isRemove) //添加
                        {
                            await dbContext.RolePermissionRelations.AddAsync(new RolePermissionRelation() { RoleId = request.RoleId, PermissionId = pid });
                        }
                        if (rpr != null && isRemove) //删除
                        {
                            dbContext.RolePermissionRelations.Remove(rpr);
                        }
                    }
                    if (await dbContext.SaveChangesAsync() < 0)
                    {
                        throw new Exception(isRemove ? "解绑成功" : "绑定成功");
                    }
                    return isRemove ? "解绑成功":"绑定成功";
                }
                catch (Exception)
                {
                    throw new Exception( isRemove ? "部分解绑失败":"部分绑定失败");
                }
            }
        }

        /// <summary>
        /// 用户角色绑定
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> UserRoleBind(UserRoleBindReq request, bool isRemove = false)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                try
                {
                    foreach (var rid in request.RoleIds)
                    {
                        var urr = await dbContext.UserRoleRelations.FirstOrDefaultAsync(rpr => rpr.UserId == request.UserId && rpr.RoleId == rid);
                        if (urr == null && !isRemove) //添加
                        {
                            await dbContext.UserRoleRelations.AddAsync(new UserRoleRelation() { UserId = request.UserId, RoleId = rid });
                        }
                        if (urr != null && isRemove) //删除
                        {
                            dbContext.UserRoleRelations.Remove(urr);
                        }
                    }
                    if (await dbContext.SaveChangesAsync() < 0)
                    {
                        throw new Exception(isRemove ? "解绑成功" : "绑定成功");
                    }
                    return isRemove ? "解绑成功" : "绑定成功";
                }
                catch (Exception)
                {
                    throw new Exception(isRemove ? "部分解绑失败" : "部分绑定失败");
                }
            }
        }
    }
}
