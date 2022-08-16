using Service.IdentityService.Domain;

namespace Service.IdentityService.App.Interface
{
    public interface IPermissionApp
    {
        object[] GetPermission(string username,string password);
    }
}
