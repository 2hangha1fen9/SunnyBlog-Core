using ArticleService.App.Interface;
using ArticleService.Domain;
using ArticleService.Request;
using ArticleService.Response;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;

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
        /// 列出分区分区
        /// </summary>
        /// <returns></returns>
        public async Task<List<RegionView>> GetRegions(string? key = null,bool isAll = false)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                List<ArtRegion> regions = null;
                if (isAll)
                {
                    regions = await dbContext.ArtRegions.ToListAsync();
                }
                else
                {
                    regions = await dbContext.ArtRegions.Where(ar => ar.Status == 1).ToListAsync();
                }

                //深度过滤条件,会过滤出相关的分区(线性的)
                if (!string.IsNullOrWhiteSpace(key))
                {
                    var result = new List<ArtRegion>();
                    RecursiveFilter(regions.ToList(),result, key);
                    return result.MapToList<RegionView>();
                }
                return regions.Where(c => c.ParentId == null).MapToList<RegionView>();
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
                    region.ParentId = request.ParentId.HasValue && request.ParentId.Value != 0 && request.ParentId != request.Id ? request.ParentId.Value : null;
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

        /// <summary>
        /// 递归搜索所有分区
        /// </summary>
        /// <param name="regions">待搜索的关键字</param>
        /// <param name="key">搜索关键字</param>
        /// <param name="result">搜索结果</param>
        /// <param name="findState">当前搜索状态</param>
        /// <returns></returns>
        public void RecursiveFilter(List<ArtRegion> regions,List<ArtRegion> result, string key,bool findState = false)
        {
            //搜索链被切断终止搜索
            if (regions == null)
            {
                return;
            }
            foreach (var region in regions)
            {
                //根元素没有被搜索才能进行搜索
                if (region.Status != 200 && (key.Equals(region.Name, StringComparison.OrdinalIgnoreCase) || key.Equals(region.Parent?.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Add(new ArtRegion
                    {
                        Id = region.Id,
                        Name = region.Name,
                        Description = region.Description,
                        Status = region.Status
                    });

                    //如果当前元素为查询到的根元素,则标记为已搜索
                    if (key.Equals(region.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        region.Status = 200;
                    }
                    findState = true; //匹配到了目标,则可以替换搜索关键字,深度搜索子元素
                }
                else
                {
                    findState = false;
                }

                //如果有子集分区则继续查询
                if (region?.InverseParent?.Count > 0)
                {
                    //如果当前为元素已搜索则切断搜索链
                    RecursiveFilter(region.Status != 200 ? region.InverseParent.ToList() : null, result, findState ? region.Name : key, findState);
                }
            }
        }
    }
}
