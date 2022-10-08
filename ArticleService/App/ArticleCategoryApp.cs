using ArticleService.App.Interface;
using ArticleService.Domain;
using ArticleService.Request;
using ArticleService.Response;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.App
{
    public class ArticleCategoryApp :IArticleCategoryApp
    {
        private readonly IDbContextFactory<ArticleDBContext> contextFactory;

        public ArticleCategoryApp(IDbContextFactory<ArticleDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// 更新分类
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> UpdateCategory(UpdateCategoryReq request, int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                try
                {
                    var category = dbContext.ArtCategories.FirstOrDefault(t => t.UserId == uid && t.Id == request.Id);
                    if (category != null)
                    {
                        category.Name = request.Name ?? category.Name;
                        category.ParentId = request.ParentId.HasValue && request.ParentId.Value != 0 ? request.ParentId.Value : null;
                        dbContext.Entry(category).State = EntityState.Modified;
                        await dbContext.SaveChangesAsync();
                    }
                    return "修改成功";
                }
                catch (Exception)
                {
                    throw new Exception("修改失败");
                }
            }
        }

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uid"></param>
        public async Task<string> CreateCategory(AddCategoryReq request, int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var category = await dbContext.ArtCategories.FirstOrDefaultAsync(t => t.UserId == uid && request.Name == t.Name && t.ParentId == request.ParentId);
                if(category != null)
                {
                    throw new Exception("目录已存在");
                }
                category = request.MapTo<ArtCategory>();
                category.UserId = uid;
                dbContext.ArtCategories.Add(category);
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("添加失败");
                }
                return $"{category.Id}";
            }
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uid"></param>
        public async Task<string> DeleteCategory(int cid, int uid)
        {
            try
            {
                using (var dbContext = contextFactory.CreateDbContext())
                {
                    //查询分类
                    var category = await dbContext.ArtCategories.FirstOrDefaultAsync(c => c.Id == cid && c.UserId == uid);
                    dbContext.ArtCategories.Remove(category);
                    if (await dbContext.SaveChangesAsync() < 0)
                    {
                        throw new Exception("删除失败");
                    }
                    return "删除成功";
                }
            }
            catch (Exception)
            {
                throw new Exception("目录下还有数据(包含回收站)");
            }
            
        }

        /// <summary>
        /// 根据用户ID查询分类信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<CategoryView>> GetUserCategory(int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var category = await dbContext.ArtCategories.Where(c => c.UserId == uid).ToListAsync();
                return category.Where(c => c.ParentId == null).MapToList<CategoryView>();
            }
        }

    }
}
