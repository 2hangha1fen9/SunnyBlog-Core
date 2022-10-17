using ArticleService.Request;
using ArticleService.Response;
using Infrastructure;

namespace ArticleService.App.Interface
{
    public interface IArticleTagApp
    {
        public void UpdateArticleTag(int articleId, List<int> tagIds);
        public void AddArticleTag(int articleId, List<int> tagId);
        public Task<List<TagView>> GetAllTags();
        public Task<List<TagView>> GetPublicTags(List<SearchCondition>? condidtion = null);
        public Task<List<TagView>> GetUserTags(int uid);
        public Task<string> UpdateTag(UpdateTagReq request, int uid);
        public Task<string> DeletelTag(List<DelTagReq> request, int uid);
        public Task<string> CreateTag(AddTagReq request, int uid);
        public Task<string> DeletelTag(List<DelTagReq> request);
        public Task<string> UpdateTag(UpdateTagReq request);
    }
}
