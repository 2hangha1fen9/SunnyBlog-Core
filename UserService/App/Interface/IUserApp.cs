using UserService.Request;
using UserService.Response;

namespace UserService.App.Interface
{
    public interface IUserApp
    {
        Task<UserView> GetUserById(int id);
        Task<List<UserView>> GetUsers(int pageIndex, int pageSize,SearchCondition[] condidtion);
        Task<string> UserRegister(UserRegisterReq request);
        Task<string> ChangePassword(ForgetPasswordReq request);
        Task<string> ChangeUser(UpdateUserReq request,int id);
        Task<string> BindAccount(BindAccountReq requests,int id);
        Task<string> UbindAccount(UbindAccountReq requests,int id);
    }
}
