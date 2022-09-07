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
        public async Task<List<RegionView>> GetRegions(bool isAll = false)
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
                    region.ParentId = request.ParentId.HasValue && request.ParentId.Value != 0 ? request.ParentId.Value : null;
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

        //
        public bool IsValidUpdate(ArtRegion region,int parentId)
        {
            if(region.Id == region.ParentId.Value)
            {
                return false;
            }
            else
            {
                if (region.InverseParent.Count > 0)
                {
                    foreach (var item in region.InverseParent)
                    {
                        return IsValidUpdate(item, parentId);
                    }
                }

            }

        }
    }
}
