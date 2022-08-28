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
        /// 添加文章分类
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="articleId"></param>
        /// <param name="categoryIds"></param>
        /// <exception cref="Exception"></exception>
        public async void AddArticleCategory(int uid, int articleId, List<int> categoryIds)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                if (categoryIds != null)
                {
                    //查询文章
                    var article = dbContext.Articles.FirstOrDefaultAsync(a => a.Id == articleId && a.UserId == uid);
                    //查询用户所有分类
                    var userCategory = await dbContext.Categories.Where(c => c.UserId == uid).ToListAsync();
                    foreach (var cid in categoryIds)
                    {
                        if (userCategory.FirstOrDefault(uc => uc.Id == cid) != null)
                        {
                            dbContext.ArtCategories.Add(new ArtCategory()
                            {
                                CategoryId = cid,
                                ArticleId = articleId
                            });
                        }
                    }
                    if (await dbContext.SaveChangesAsync() < 0)
                    {
                        throw new Exception("文章分类添加失败");
                    }
                }
               
            }
        }

        /// <summary>
        /// 更新文章分类
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="articleId"></param>
        /// <param name="categoryIds"></param>
        public async void UpdateArticleCategory(int uid, int articleId, List<int> categoryIds)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询用户所有分类
                var uCategory = await dbContext.Categories.Where(t => t.UserId == uid).ToListAsync();
                //查询文章所有分类
                var aCategorys = await dbContext.ArtCategories.Where(at => at.ArticleId == articleId).ToListAsync();
                foreach (var c in aCategorys)//先删除当前文章所有分类
                {
                    dbContext.Entry(c).State = EntityState.Deleted;
                }
                foreach (var cid in categoryIds)//添加最新分类信息
                {
                    if (uCategory.FirstOrDefault(uc => uc.Id == cid) != null) //用户拥有这个分类
                    {
                        dbContext.Entry(new ArtCategory()
                        {
                            ArticleId = articleId,
                            CategoryId = cid,
                        }).State = EntityState.Added;
                    }
                }
                //保存修改
                await dbContext.SaveChangesAsync();
            }
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
                    var category = dbContext.Categories.FirstOrDefault(t => t.UserId == uid && t.Id == request.Id);
                    if (category != null)
                    {
                        category.Name = request.Name ?? category.Name;
                        category.ParentId = request.ParentId;
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
        public async Task<string> AddCategory(AddCategoryReq request, int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var category = request.MapTo<Category>();
                category.UserId = uid;
                dbContext.Categories.Add(category);
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("添加失败");
                }
                return "添加成功";
            }
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uid"></param>
        public async Task<string> DeletelCategory(List<DelCategoryReq> request, int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询用户所有分类
                var category = await dbContext.Categories.Where(c => c.UserId == uid).ToListAsync();
                foreach (var cid in request)
                {
                    var c = category.FirstOrDefault(a => a.Id == cid.Id && a.UserId == uid);
                    if (c != null)
                    {
                        dbContext.Entry(c).State = EntityState.Deleted;
                    }
                }
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("删除失败");
                }
                return "删除成功";
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
                var category = await dbContext.Categories.Where(c => c.UserId == uid).ToListAsync();
                return category.Where(c => c.ParentId == null).MapToList<CategoryView>();
            }
        }

    }
}
