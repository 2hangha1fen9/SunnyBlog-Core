using Infrastructure;
using UserService.Request;
using UserService.Request.Request;
using UserService.Response;

namespace UserService.App.Interface
{
    public interface IUserApp
    {
        Task<UserView> GetUserById(int id);
        Task<PageList<UserView>> GetUsers(List<SearchCondition>? condidtion,int pageIndex,int pageSize);
        Task<UserDetailView> GetUserDetailById(int id);
        Task<PageList<UserDetailView>> GetUserDetails(List<SearchCondition>? condition,int pageIndex, int pageSize);
        Task<string> UserRegister(UserRegisterReq request);
        Task<string> ChangePassword(ForgetPasswordReq request);
        Task<string> ChangeUser(ChangeUserReq request,int id);
        Task<string> BindAccount(BindAccountReq requests,int id);
        Task<string> UbindAccount(UbindAccountReq requests,int id);
        Task<string> UpdateUserDetail(UpdateUserReq request);
        Task<string> AddUser(AddUserReq request);
        Task<string> DelUser(List<DelUserReq> requests);
    }
}
