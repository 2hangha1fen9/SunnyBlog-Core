using Infrastructure;
using UserService.Response;

namespace UserService.App.Interface
{
    public interface IFollowApp
    {
        Task<string> FollowSb(int id,int sbId);
        Task<PageList<FollowView>> FollowList(int id, bool fans, List<SearchCondition>? condidtion, int pageIndex, int pageSize);
        Task<List<FollowView>> GetFollowMessage(int uid);
        Task<string> DeleteFollowMessage(int uid, int fid, bool isAll);
        Task<bool> FollowStatus(int id, int sbId);
    }
}
