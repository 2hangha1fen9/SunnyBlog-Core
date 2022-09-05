using IdentityService.Domain;
using IdentityService.Request;
using IdentityService.Response;
using Infrastructure;

namespace Service.IdentityService.App.Interface
{
    public interface IPermissionApp
    {
        Task<object[]> GetPermission(string username,string password);
        Task<object[]> GetPermission(string username);
        Task<PermissionView> GetPermissionById(int id);
        Task<PageList<PermissionView>> GetPermissionsByUserId(int id,int pageIndex, int pageSize);
        Task<PageList<PermissionView>> GetPermissionsByRoleId(int id,int pageIndex, int pageSize);
        Task<PageList<PermissionView>> ListPermission(List<SearchCondition> condidtion,int pageIndex, int pageSize);
        Task<string> ChangePermission(ModifyPermissionReq request);
        Task<string> AddPermission(AddPermissionReq request);
        Task<string> DelPermission(List<DelPermissionReq> request);
    }
}
