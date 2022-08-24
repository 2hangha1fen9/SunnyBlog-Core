using ArticleService.Request;
using ArticleService.Response;
using Infrastructure;

namespace ArticleService.App.Interface
{
    public interface IArticleApp
    {
        Task<ArticleView> GetArticle(int id);
        Task<PageList<ArticleView>> GetArticleList(List<SearchCondition> condidtion, int pageIndex, int pageSize);
        Task<PageList<ArticleListView>> GetRowList(List<SearchCondition> condidtion, int pageIndex, int pageSize);
        Task<PageList<ArticleListView>> GetRowList(List<SearchCondition> condidtion, int uid, int pageIndex, int pageSize);
        Task<string> RemoveArticle(List<DelArticleReq> request);
        Task<string> RemoveArticle(List<DelArticleReq> request, int uid);
        Task<string> EditorArticle(EditorArticleReq request);
        Task<string> EditorArticle(EditorArticleReq request, int uid);
        Task<string> PublishArticle(PublishArticleReq request,int uid);
    }
}
