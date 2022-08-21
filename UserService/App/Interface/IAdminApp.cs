using UserService.Request;
using UserService.Request.Admin;
using UserService.Request.Request.Admin;
using UserService.Response;

namespace UserService.App.Interface
{
    public interface IAdminApp
    {
        Task<UserDetailView> GetUserDetailById(int id);
        Task<List<UserDetailView>> GetUserDetails(int pageIndex, int pageSize,SearchCondition[] condition);
        Task<string> ChangeUserDetail(ModifyUserReq request);
        Task<string> AddUser(AddUserReq request);
        Task<string> DelUser(List<DelUserReq> requests);
    }
}
