using CommentService.Response;

namespace CommentService.App.Interface
{
    public interface IViewApp
    {
        Task AddArticleViewCount(int aid,int? uid = null,string? ip = null);
        Task<int> GetArticleViewCount(int aid);
        Task<List<UserViewHistory>> GetUserView(int uid);
        Task<List<UserViewHistory>> GetViewList();
    }
}
