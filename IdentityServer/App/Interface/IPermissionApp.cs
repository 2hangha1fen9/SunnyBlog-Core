using Service.IdentityService.Domain;

namespace Service.IdentityService.App.Interface
{
    public interface IPermissionApp
    {
        Task<object[]> GetPermission(string username,string password);
    }
}
