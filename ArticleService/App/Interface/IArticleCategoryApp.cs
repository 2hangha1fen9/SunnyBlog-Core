using ArticleService.Domain;
using ArticleService.Request;
using ArticleService.Response;

namespace ArticleService.App.Interface
{
    public interface IArticleCategoryApp
    {
        public Task<List<CategoryView>> GetUserCategory(int uid);
        public Task<string> UpdateCategory(UpdateCategoryReq request,int uid);
        public Task<string> DeleteCategory(int cid,int uid);
        public Task<string> CreateCategory(AddCategoryReq request,int uid);
    }
}
