using IdentityService.Request;

namespace IdentityService.App.Interface
{
    public interface IRelationApp
    {
        Task<string> RolePermissionBind(RolePermissionBindReq request);
        Task<string> UserRoleBind(UserRoleBindReq request);
    }
}
