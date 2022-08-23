using IdentityService.Request;
using IdentityService.Response;
using Infrastructure;

namespace IdentityService.App.Interface
{
    public interface IRoleApp
    {
        Task<RoleView> GetRoleById(int id);
        Task<List<RoleView>> GetRoleByUserId(int id);
        Task<List<RoleView>> GetRoleByPermissionId(int id);
        Task<List<RoleView>> ListRole(SearchCondition[] condidtion);
        Task<string> ChangeRole(ModifyRoleReq request);
        Task<string> AddRole(AddRoleReq request);
        Task<string> DelRole(List<DelRoleReq> request);
    }
}
