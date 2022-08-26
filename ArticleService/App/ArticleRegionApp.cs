using ArticleService.App.Interface;
using ArticleService.Domain;
using ArticleService.Request;
using ArticleService.Response;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.App
{
    public class ArticleRegionApp : IArticleRegionApp
    {
        private readonly IDbContextFactory<ArticleDBContext> contextFactory;

        public ArticleRegionApp(IDbContextFactory<ArticleDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// 创建分区
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> CreateRegion(AddRegionReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var region = request.MapTo<ArtRegion>();
                dbContext.ArtRegions.Add(region);
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("分区创建失败");
                }
                return "创建成功";
            }
        }

        /// <summary>
        /// 删除分区
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> DeletelRegion(List<DelRegionReq> request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var regions = request.MapToList<ArtRegion>();
                dbContext.ArtRegions.RemoveRange(regions);
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("分区创建失败");
                }
                return "删除成功";
            }
        }

        /// <summary>
        /// 获取所有分区
        /// </summary>
        /// <returns></returns>
        public async Task<List<RegionView>> GetRegions()
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var regions = await dbContext.ArtRegions.ToListAsync();
                return regions.MapToList<RegionView>();
            }
        }

        /// <summary>
        /// 更新分区
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> UpdateRegion(UpdateRegionReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var region = await dbContext.ArtRegions.FirstOrDefaultAsync(r => r.Id == request.Id);
                if (region != null)
                {
                    region.Name = request.Name ?? region.Name;
                    region.Description = request.Description ?? region.Description;
                    region.ParentId = request.ParentId ?? region.ParentId;
                    region.Status = request.Status ?? region.Status;
                    //保存修改
                    dbContext.Entry(region).State = EntityState.Modified;
                    if (await dbContext.SaveChangesAsync() < 0)
                    {
                        throw new Exception("更新失败");
                    }
                    return "更新成功";
                }
                throw new Exception("没有找到这分区");
            }
        }
    }
}
