using ArticleService.Domain;
using ArticleService.Request;
using ArticleService.Response;
using Infrastructure;
using System.Linq.Expressions;

namespace ArticleService.App.Interface
{
    public interface IArticleApp
    {
        Task<ArticleView> GetArticle(int id, bool allScope = false);
        Task<PageList<ArticleListView>> GetArticleList(List<SearchCondition>? condidtion, Expression<Func<Article, bool>> predict, int pageIndex, int pageSize);
        Task<string> RemoveArticle(List<DelArticleReq> request);
        Task<string> RemoveArticle(List<DelArticleReq> request, int uid);
        Task<string> EditorArticle(EditorArticleReq request, int? uid);
        Task<string> PublishArticle(PublishArticleReq request,int uid);
    }
}
