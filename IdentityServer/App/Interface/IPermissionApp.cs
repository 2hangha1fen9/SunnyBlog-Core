using Service.IdentityService.Domain;

namespace Service.IdentityService.App.Interface
{
    public interface IPermissionApp
    {
        string GetPermission(User user);
    }
}
