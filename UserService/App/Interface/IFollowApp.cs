using UserService.Response;

namespace UserService.App.Interface
{
    public interface IFollowApp
    {
        Task<string> FollowSb(int id,int sbId);
        Task<string> CancelFollowSb(int id, int sbId);
        Task<List<FollowView>> FollowList(int id);
        Task<bool> FollowStatus(int id, int sbId);
    }
}
