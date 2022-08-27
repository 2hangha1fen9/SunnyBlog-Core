using CommentService.Request;
using CommentService.Response;
using Infrastructure;

namespace CommentService.App.Interface
{
    public interface ICommentApp
    {
        Task<string> CommentArticle(CommentReq request,int uid);
        Task<PageList<CommentView>> GetArticleComment(int aid,List<SearchCondition> condidtion, int pageIndex, int pageSize);
        Task<PageList<CommentListView>> GetCommentList(List<SearchCondition> condidtion, int pageIndex, int pageSize);
        Task<PageList<CommentListView>> GetUserCommentList(int uid, List<SearchCondition> condidtion, int pageIndex, int pageSize);
        Task<PageList<CommentListView>> GetMyCommentList(int uid, List<SearchCondition> condidtion, int pageIndex, int pageSize);
        Task<string> DeleteArticleComment(int cid, int uid);
        Task<string> DeleteComment(int cid, int uid);
        Task<string> DeleteComment(int cid);
        Task<string> ReadArticle(int cid, int uid);
    }
}
