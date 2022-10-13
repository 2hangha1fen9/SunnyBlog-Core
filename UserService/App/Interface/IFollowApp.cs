using Infrastructure;
using UserService.Response;

namespace UserService.App.Interface
{
    public interface IFollowApp
    {
        Task<string> FollowSb(int id,int sbId);
        Task<List<FollowView>> FollowList(List<SearchCondition> condidtion, int id, bool fans = false);
        Task<List<FollowView>> GetFollowMessage(int uid);
        Task<string> DeleteFollowMessage(int uid, int fid, bool isAll);
        Task<bool> FollowStatus(int id, int sbId);
    }
}
