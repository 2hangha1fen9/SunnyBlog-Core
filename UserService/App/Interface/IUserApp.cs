using UserService.Request;
using UserService.Response;

namespace UserService.App.Interface
{
    public interface IUserApp
    {
        Task<UserView> GetUserById(int id);
        Task<UserDetailView> GetUserDetailById(int id);
        Task<List<UserView>> GetUsers();
        Task<List<UserDetailView>> GetUserDetails();
        Task<string> UserRegister(UserRegisterReq request); 
        Task<string> SendMessage(string phone)
    }
}
