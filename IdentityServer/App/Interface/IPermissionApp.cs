using IdentityService.Domain;
using IdentityService.Request;
using IdentityService.Response;
using Infrastructure;

namespace Service.IdentityService.App.Interface
{
    public interface IPermissionApp
    {
        Task<object[]> GetPermission(string username,string password);
        Task<PermissionView> GetPermissionById(int id);
        Task<List<PermissionView>> GetPermissionsByUserId(int id);
        Task<List<PermissionView>> GetPermissionsByRoleId(int id);
        Task<List<PermissionView>> ListPermission(SearchCondition[] condidtion);
        Task<string> ChangePermission(ModifyPermissionReq request);
        Task<string> AddPermission(AddPermissionReq request);
        Task<string> DelPermission(List<DelPermissionReq> request);
    }
}
