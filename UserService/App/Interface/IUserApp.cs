using UserService.Response;

namespace UserService.App.Interface
{
    public interface IUserApp
    {
        UserView GetUserById(int id);
        List<UserView> GetUsers();
    }
}
