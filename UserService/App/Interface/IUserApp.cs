using UserService.Response;

namespace UserService.App.Interface
{
    public interface IUserApp
    {
        Task<UserView> GetUserById(int id);
        Task<List<UserView>> GetUsers();
    }
}
