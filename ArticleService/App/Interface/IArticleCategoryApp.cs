using ArticleService.Domain;
using ArticleService.Request;
using ArticleService.Response;

namespace ArticleService.App.Interface
{
    public interface IArticleCategoryApp
    {
        public void AddArticleCategory(int uid, int articleId, List<int> categoryIds);
        public void UpdateArticleCategory(int uid, int articleId, List<int> categoryIds);
        public Task<List<CategoryView>> GetUserCategory(int uid);
        public Task<string> UpdateCategory(UpdateCategoryReq request,int uid);
        public Task<string> DeletelCategory(List<DeleteCategoryReq> request,int uid);
        public Task<string> AddCategory(AddCategoryReq request,int uid);
    }
}
