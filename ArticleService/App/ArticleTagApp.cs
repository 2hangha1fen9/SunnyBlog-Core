using ArticleService.App.Interface;
using ArticleService.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.App
{
    public class ArticleTagApp : IArticleTagApp
    {
        private readonly IDbContextFactory<ArticleDBContext> contextFactory;

        public ArticleTagApp(IDbContextFactory<ArticleDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// 添加文章标签
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="tagId"></param>
        /// <exception cref="NotImplementedException"></exception>
        public async void AddArticleTag(int articleId, List<int> tagIds)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询所有标签
                var tags = await dbContext.Tags.ToListAsync();
                foreach (var tid in tagIds)
                {
                    if (tags.FirstOrDefault(t => t.Id == tid) != null)
                    {
                        await dbContext.ArticleTags.AddAsync(new ArticleTag()
                        {
                            ArticleId = articleId,
                            TagId = tid
                        });
                    }
                }
                //保存修改
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("文章标签添加失败");
                }
            }
        }

        /// <summary>
        /// 更新文章标签
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="tagIds"></param>
        public async void UpdateArticleTag(int articleId, List<int> tagIds)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询文章所有标签
                var aTags = await dbContext.ArticleTags.Where(at => at.ArticleId == articleId).ToListAsync();
                //查询所有标签
                var tags = await dbContext.Tags.ToListAsync();

                foreach (var t in aTags)//先删除当前文章所有标签
                {
                    dbContext.Entry(t).State = EntityState.Deleted;
                }
                foreach (var tid in tagIds)//添加最新标签
                {
                    dbContext.Entry(new ArticleTag()
                    {
                        ArticleId = articleId,
                        TagId = tid
                    }).State = EntityState.Added;
                }
                //保存修改
                if(await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("文章标签更新失败");
                }
            }
        }
    }
}
