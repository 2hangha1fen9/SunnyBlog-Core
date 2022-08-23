using IdentityService.Request;

namespace IdentityService.App.Interface
{
    public interface IRelationApp
    {
        Task<string> RolePermissionBind(RolePermissionBindReq request, bool isRemove = false);
        Task<string> UserRoleBind(UserRoleBindReq request, bool isRemove = false);
    }
}
