using ArticleService.Request;
using ArticleService.Response;

namespace ArticleService.App.Interface
{
    public interface IArticleRegionApp
    {
        public Task<List<RegionView>> GetRegions(bool isAll = false);
        public Task<string> UpdateRegion(UpdateRegionReq request);
        public Task<string> DeletelRegion(List<DelRegionReq> request);
        public Task<string> CreateRegion(AddRegionReq request);
    }
}
