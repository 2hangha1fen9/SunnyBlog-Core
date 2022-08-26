using IdentityService.App.Interface;
using IdentityService.Domain;
using IdentityService.Request;
using IdentityService.Response;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.App
{
    public class RoleApp : IRoleApp
    {
        private readonly IDbContextFactory<AuthDBContext> contextFactory;

        public RoleApp(IDbContextFactory<AuthDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> AddRole(AddRoleReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                if(await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == request.Name) != null)
                {
                    throw new Exception("角色已存在");
                }

                var role = request.MapTo<Role>();
                await dbContext.Roles.AddAsync(role);
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
        /// 修改角色
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> ChangeRole(ModifyRoleReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var role = await dbContext.Roles.FirstOrDefaultAsync(p => p.Id == request.Id);
                if (role != null)
                {
                    role.Name = request.Name ?? role.Name;
                    role.Status = request.Status ?? role.Status;
                    role.UpdateTime = DateTime.Now;
                    dbContext.Entry(role).State = EntityState.Modified;
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
                    throw new Exception("角色信息异常");
                }
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> DelRole(List<DelRoleReq> requests)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var roles = requests.MapToList<Role>();
                dbContext.Roles.RemoveRange(roles);
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
        /// 根据ID获取角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RoleView> GetRoleById(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var role = await context.Roles.FirstOrDefaultAsync(r => r.Id == id);
                return role.MapTo<RoleView>();
            }
        }

        /// <summary>
        /// 根据权限ID获取角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PageList<RoleView>> GetRoleByPermissionId(int id, int pageIndex, int pageSize)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var roles = context.RolePermissionRelations.Where(rpr => rpr.PermissionId == id).Select(r => new Role()
                {
                    Id = r.RoleId,
                    Name = r.Role.Name,
                    Status = r.Role.Status,
                    CreateTime = r.Role.CreateTime,
                    UpdateTime = r.Role.UpdateTime,
                });
                var rolePage = new PageList<RoleView>();
                roles = rolePage.Pagination(pageIndex, pageSize, roles);
                rolePage.Page = (await roles.ToListAsync()).MapToList<RoleView>();
                return rolePage;
            }
        }

        /// <summary>
        /// 根据用户ID获取角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PageList<RoleView>> GetRoleByUserId(int id, int pageIndex, int pageSize)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var roles = context.UserRoleRelations.Where(urr => urr.UserId == id).Select(r => new Role()
                {
                    Id = r.Id,
                    Name = r.Role.Name,
                    Status = r.Role.Status,
                    UpdateTime = r.Role.UpdateTime,
                    CreateTime = r.Role.CreateTime
                });
                var rolePage = new PageList<RoleView>();
                roles = rolePage.Pagination(pageIndex, pageSize, roles);
                rolePage.Page = (await roles.ToListAsync()).MapToList<RoleView>();
                return rolePage;
            }
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="condidtion"></param>
        /// <returns></returns>
        public async Task<PageList<RoleView>> ListRole(List<SearchCondition> condidtion, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var roles = dbContext.Roles.AsQueryable();
                if (condidtion.Count > 0)
                {
                    foreach (var con in condidtion)
                    {
                        roles = "Id".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? roles.Where(r => r.Id == Convert.ToInt32(con.Value)) : roles;
                        roles = "Name".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? roles.Where(r => r.Name.Contains(con.Value)) : roles;
                        roles = "Status".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? roles.Where(r => r.Status == Convert.ToInt32(con.Value)) : roles;
                    }
                }
                var rolePage = new PageList<RoleView>();
                roles = rolePage.Pagination(pageIndex, pageSize, roles);
                rolePage.Page = (await roles.ToListAsync()).MapToList<RoleView>();
                return rolePage;
            }
        }
    }
}
