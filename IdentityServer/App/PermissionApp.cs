using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Service.IdentityService.App.Interface;
using Service.IdentityService.Domain;

namespace Service.IdentityService.App
{
    public class PermissionApp : IPermissionApp
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly AuthDBContext context;
        public PermissionApp(AuthDBContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 根据用户Id获取权限
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GetPermission(User user)
        {
            //查询用户权限
            var permissions = context.Set<Permission>().FromSqlRaw($"select * from Permission as p where p.id in(select permissionId from RolePermissionRelation as rp where rp.roleId in(select roleId from UserRoleRelation as ur,Role r where ur.roleId = r.id and ur.userId = {user.Id} and r.status = 1)) and status = 1").Select(s => new
            {
                Controller = s.Controller,
                Action = s.Action,
            });
            //序列化Json
            var str = JsonConvert.SerializeObject(permissions);
            return str;
        }
    }
}
