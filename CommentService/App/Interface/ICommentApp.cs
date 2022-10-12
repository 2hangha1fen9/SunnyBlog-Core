using CommentService.Request;
using CommentService.Response;
using Infrastructure;

namespace CommentService.App.Interface
{
    public interface ICommentApp
    {
        Task<string> CommentArticle(CommentReq request,int uid);
        Task<PageList<CommentView>> GetArticleComment(int aid,List<SearchCondition>? condidtion, int pageIndex, int pageSize);
        Task<PageList<CommentListView>> GetCommentList(List<SearchCondition>? condidtion, int pageIndex, int pageSize);
        Task<PageList<CommentListView>> GetUserCommentList(int uid, List<SearchCondition>? condidtion, int pageIndex, int pageSize);
        Task<PageList<CommentListView>> GetMyCommentList(int uid, List<SearchCondition>? condidtion, int pageIndex, int pageSize);
        Task<string> DeleteArticleComment(int cid, int uid);
        Task<string> DeleteComment(int cid, int uid);
        Task<string> DeleteComment(int[] cids);
        Task<List<CommentListView>> GetUserUnreadComment(int uid);
        Task<string> ReadComment(int cid, int uid,bool isAll = false);
        Task<string> AllowComment(int cid, int? uid);
        Task<int> GetArticleCommentCount(int aid);
    }
}
