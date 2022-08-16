using Service.IdentityService.Domain;

namespace Service.IdentityService.App.Interface
{
    public interface IUserApp
    {
        User GetUser(string account, string password);
    }
}
