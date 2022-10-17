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
                if (tagIds != null)
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
                return $"{tag.Id}";
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
        /// 删除标签(需要用户ID)
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
        /// 获取所有标签
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<TagView>> GetAllTags()
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var tags = await dbContext.Tags.Select(t => new
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    Name = t.Name,
                    Color = t.Color,
                    IsPrivate = t.IsPrivate,
                    ArticleCount = t.ArticleTags.Count()

                }).ToListAsync();
                return tags.MapToList<TagView>();
            }
        }

        /// <summary>
        /// 获取所有公共标签
        /// </summary>
        /// <returns></returns>
        public async Task<List<TagView>> GetPublicTags(List<SearchCondition>? condidtion = null)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var tags = await dbContext.Tags.Where(t => t.IsPrivate == 1).Select(t => new
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    Name = t.Name,
                    Color = t.Color,
                    IsPrivate = t.IsPrivate,
                    ArticleCount = t.ArticleTags.Count()
                }).ToListAsync();
                //条件筛选
                if(condidtion?.Count > 0)
                {
                    foreach (var con in condidtion)
                    {
                        tags = "Name".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? tags.Where(a => a.Name.Contains(con.Value, StringComparison.OrdinalIgnoreCase)).ToList() : tags;
                        //排序
                        if (con.Sort != 0)
                        {
                            if ("ArticleCount".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                            {
                                if (con.Sort == -1)
                                {
                                    tags = tags.OrderByDescending(a => a.ArticleCount).ToList();
                                }
                                else
                                {
                                    tags = tags.OrderBy(a => a.ArticleCount).ToList();
                                }
                            }
                        }
                    }
                }
                
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
                var tags = await dbContext.Tags.Where(t => t.UserId == uid || t.IsPrivate == 1).Select(t => new
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    Name = t.Name,
                    Color = t.Color,
                    IsPrivate = t.IsPrivate,
                    ArticleCount = t.ArticleTags.Count(t => t.Article.UserId == uid)

                }).ToListAsync();
                return tags.MapToList<TagView>();
            }
        }

        /// <summary>
        /// 修改标签
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
                    tags.UserId = request.UserId ?? tags.UserId;
                    tags.IsPrivate = request.IsPrivate ?? tags.IsPrivate;
                    await dbContext.SaveChangesAsync();
                    return "修改成功";
                }
                else
                {
                    throw new Exception("标签信息异常");
                }
            }
        }

        /// <summary>
        /// 更新标签(需要用户ID)
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
                    await dbContext.SaveChangesAsync();
                     return "修改成功";
                }
                else
                {
                    throw new Exception("标签信息异常");
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
                    //判断标签是否存在,存在则添加
                    if (tags.FirstOrDefault(t => t.Id == tid) != null)
                    {
                        dbContext.Entry(new ArticleTag()
                        {
                            ArticleId = articleId,
                            TagId = tid
                        }).State = EntityState.Added;
                    }
                }
                //保存修改
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
