using ArticleService.App.Interface;
using ArticleService.Domain;
using ArticleService.Request;
using ArticleService.Response;
using Infrastructure;
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
        /// 创建标签
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> CreateTag(AddTagReq request, int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                if (await dbContext.Tags.FirstOrDefaultAsync(t => t.Name == request.Name) != null)
                {
                    throw new Exception("标签已存在");
                }
                var tag = request.MapTo<Tag>();
                tag.UserId = uid;
                dbContext.Tags.Add(tag);
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("标签添加失败");
                }
                return "添加成功";
            }
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> DeletelTag(List<DelTagReq> request, int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询用户所有标签
                var uTag = await dbContext.Tags.Where(c => c.UserId == uid).ToListAsync();
                foreach (var tid in request)
                {
                    var t = uTag.FirstOrDefault(a => a.Id == tid.Id && a.UserId == uid);
                    if (t != null)
                    {
                        dbContext.Entry(t).State = EntityState.Deleted;
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
        /// 获取所有标签(管理)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<TagView>> GetAllTags()
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var tags = await dbContext.Tags.ToListAsync();
                return tags.MapToList<TagView>();
            }
        }

        /// <summary>
        /// 获取所有公共标签
        /// </summary>
        /// <returns></returns>
        public async Task<List<TagView>> GetPublicTags()
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var tags = await dbContext.Tags.Where(t => t.IsPrivate == 0).ToListAsync();
                return tags.MapToList<TagView>();
            }
        }

        /// <summary>
        /// 获取用户标签
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<TagView>> GetUserTags(int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var tags = await dbContext.Tags.Where(t => t.UserId == uid).ToListAsync();
                return tags.MapToList<TagView>();
            }
        }

        /// <summary>
        /// 修改标签(管理)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> UpdateTag(UpdateTagReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var tags = dbContext.Tags.FirstOrDefault(t => t.Id == request.Id);
                if (tags != null)
                {
                    tags.Name = request.Name ?? tags.Name;
                    tags.Color = request.Color ?? tags.Color;
                    tags.IsPrivate = request.IsPrivate ?? tags.IsPrivate;

                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        return "修改成功";
                    }
                    else
                    {
                        throw new Exception("修改失败");
                    }
                }
                else
                {
                    throw new Exception("标签信息异常");
                }
            }
        }

        /// <summary>
        /// 更新标签
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> UpdateTag(UpdateTagReq request, int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var tags = dbContext.Tags.FirstOrDefault(t => t.UserId == uid && t.Id == request.Id);
                if (tags != null)
                {
                    tags.Name = request.Name ?? tags.Name;
                    tags.Color = request.Color ?? tags.Color;
                    tags.IsPrivate = request.IsPrivate ?? tags.IsPrivate;

                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        return "修改成功";
                    }
                    else
                    {
                        throw new Exception("修改失败");
                    }
                }
                else
                {
                    throw new Exception("标签信息异常");
                }
            }
        }

        /// <summary>
        /// 删除标签(管理)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> DeletelTag(List<DelTagReq> request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var tags = request.MapToList<Tag>();
                dbContext.Tags.RemoveRange(tags);
                if (await dbContext.SaveChangesAsync() > 0)
                {
                    return "删除成功";
                }
                else
                {
                    throw new Exception("删除失败");
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
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
