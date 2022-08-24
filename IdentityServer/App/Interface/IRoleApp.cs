using IdentityService.Request;
using IdentityService.Response;
using Infrastructure;

namespace IdentityService.App.Interface
{
    public interface IRoleApp
    {
        Task<RoleView> GetRoleById(int id);
        Task<PageList<RoleView>> GetRoleByUserId(int id, int pageIndex, int pageSize);
        Task<PageList<RoleView>> GetRoleByPermissionId(int id, int pageIndex, int pageSize);
        Task<PageList<RoleView>> ListRole(List<SearchCondition> condidtion, int pageIndex, int pageSize);
        Task<string> ChangeRole(ModifyRoleReq request);
        Task<string> AddRole(AddRoleReq request);
        Task<string> DelRole(List<DelRoleReq> request);
    }
}
