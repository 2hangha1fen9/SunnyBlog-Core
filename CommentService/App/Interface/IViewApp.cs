using CommentService.Response;
using Infrastructure;

namespace CommentService.App.Interface
{
    public interface IViewApp
    {
        Task AddArticleViewCount(int aid,int? uid = null,string? ip = null);
        Task<int> GetArticleViewCount(int aid);
        Task<int> GetUserViewCount(int uid);
        Task<PageList<UserViewHistory>> GetUserView(int uid, List<SearchCondition>? condition, int pageIndex, int pageSize);
        Task<PageList<UserViewHistory>> GetViewList(List<SearchCondition>? condition, int pageIndex, int pageSize);
    }
}
