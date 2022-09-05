using ArticleService.App.Interface;
using ArticleService.Rpc.Protos;
using ArticleService.Request;
using ArticleService.Response;
using Grpc.Net.Client;
using Infrastructure;
using Infrastructure.Consul;
using Microsoft.EntityFrameworkCore;
using ArticleService.Domain;
using static ArticleService.Rpc.Protos.gUser;
using System.Linq.Expressions;

namespace ArticleService.App
{
    public class ArticleApp : IArticleApp
    {
        private readonly IDbContextFactory<ArticleDBContext> contextFactory;
        private readonly IArticleTagApp articleTagApp;
        private readonly IArticleCategoryApp articleCategoryApp;
        private readonly gUserClient userRpc;

        public ArticleApp(IDbContextFactory<ArticleDBContext> contextFactory, IArticleTagApp articleTagApp, IArticleCategoryApp articleCategoryApp, gUserClient userRpc)
        {
            this.contextFactory = contextFactory;
            this.articleTagApp = articleTagApp;
            this.articleCategoryApp = articleCategoryApp;
            this.userRpc = userRpc;
        }

        /// <summary>
        /// 编辑文章
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> EditorArticle(EditorArticleReq request, int? uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                Article? article = null;
                //查询文章
                if (uid.HasValue)
                {
                    article = await dbContext.Articles.FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == uid && a.IsLock == 0);
                }
                else
                {
                    article = await dbContext.Articles.FirstOrDefaultAsync(a => a.Id == request.Id);
                }
                if (article != null)
                {
                    //文章更新
                    article.Title = request.Title ?? article.Title;
                    article.Summary = request.Summary ?? article.Summary;
                    article.Content = request.Content ?? article.Content;
                    article.Photo = request.Photo ?? article.Photo;
                    article.RegionId = request.RegionId ?? article.RegionId;
                    article.Status = request.Status ?? article.Status;
                    article.IsLock = request.isLock ?? article.IsLock;
                    article.CommentStatus = request.CommentStatus ?? article.CommentStatus;
                    article.UpdateTime = DateTime.Now;
                    //标签更新
                    if (request.Tags?.Count > 0)
                    {
                        articleTagApp.UpdateArticleTag(article.Id, request.Tags);
                    }
                    //分区更新
                    if (uid.HasValue && request.Categorys?.Count > 0)
                    {
                        articleCategoryApp.UpdateArticleCategory(uid.Value, article.Id, request.Categorys);
                    }
                    //保存修改
                    dbContext.Entry(article).State = EntityState.Modified;
                    if (await dbContext.SaveChangesAsync() < 0)
                    {
                        throw new Exception("更新失败");
                    }
                    else
                    {
                        return "更新成功";
                    }
                }
                else
                {
                    throw new Exception("没有找到这篇文章");
                }
            }
        }

        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ArticleView> GetArticle(int id)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //创建数据映射关系
                var article = await dbContext.Articles.Select(a => new
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    Summary = a.Summary,
                    Photo = a.Photo,
                    RegionName = a.Region.Name,
                    RegionId = a.RegionId,
                    Tags = a.ArticleTags.Where(at => at.Tag.IsPrivate == 1).Select(at => new TagView()
                    {
                        Id = at.Id,
                        UserId = at.Tag.UserId,
                        Name = at.Tag.Name,
                        Color = at.Tag.Color
                    }),
                    Categorys = a.ArtCategories.Select(c => new CategoryView()
                    {
                        Id = c.Id,
                        Name = c.Category.Name,
                        UserId = c.Category.UserId,
                        ParentId = c.Category.ParentId ?? 0,
                        InverseParent = null
                    }),
                    Status = a.Status,
                    CommentStatus = a.CommentStatus,
                    CreateTime = a.CreateTime,
                }).FirstOrDefaultAsync(a => a.Id == id && a.Status == 1);
                if (article != null)
                {
                    var adv = article.MapTo<ArticleView>();
                    return adv;
                }
                else
                {
                    throw new Exception("没有找到此文章");
                }
            }
        }

        /// <summary>
        /// 获取文章列表
        /// </summary>
        public async Task<PageList<ArticleListView>> GetArticleList(List<SearchCondition> condidtion, Expression<Func<Article, bool>> predict , int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //创建数据映射
                var article = dbContext.Articles.AsQueryable();
                //使用前置条件,判断是否是只需要用户的文章,还是用户自己个人的文章,还是所有文章
                if (predict != null)
                {
                    article = article.Where(predict);
                }
                var articleMap = article.Select(a => new
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    Nick = "",
                    Username = "",
                    Title = a.Title,
                    Summary = a.Summary,
                    Photo = a.Photo,
                    RegionName = a.Region.Name,
                    RegionId = a.RegionId,
                    Tags = a.ArticleTags.Where(at => at.Tag.IsPrivate == 1).Select(at => new TagView()
                    {
                        Id = at.Id,
                        UserId = at.Tag.UserId,
                        Name = at.Tag.Name,
                        Color = at.Tag.Color
                    }),
                    Categorys = a.ArtCategories.Select(c => new CategoryView()
                    {
                        Id = c.Id,
                        Name = c.Category.Name,
                        UserId = c.Category.UserId,
                        ParentId = c.Category.ParentId ?? 0,
                        InverseParent = null
                    }),
                    Status = a.Status,
                    IsLock = a.IsLock,
                    CommentStatus = a.CommentStatus,
                    CreateTime = a.CreateTime,
                });
                //筛选条件
                if (condidtion.Count > 0)
                {
                    foreach (var con in condidtion)
                    {
                        articleMap = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Username.Contains(con.Value)) : articleMap;
                        articleMap = "Title".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Title.Contains(con.Value)) : articleMap;
                        articleMap = "Summary".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Summary.Contains(con.Value)) : articleMap;
                        articleMap = "Region".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.RegionName.Contains(con.Value)) : articleMap;
                        articleMap = "Tag".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Tags.Where(t => t.Name.Contains(con.Value) || t.Id == Convert.ToInt32(con.Value)).Count() > 0) : articleMap;
                        articleMap = "Category".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Categorys.Where(c => c.Name.Contains(con.Value) || c.Id == Convert.ToInt32(con.Value)).Count() > 0) : articleMap;
                        articleMap = "Status".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Status == Convert.ToInt32(con.Value)) : articleMap;
                        articleMap = "IsLock".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.IsLock == Convert.ToInt32(con.Value)) : articleMap;
                        articleMap = "CommentStatus".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.CommentStatus == Convert.ToInt32(con.Value)) : articleMap;
                    }
                }
                //分页条件
                var articlePage = new PageList<ArticleListView>();
                articleMap = articlePage.Pagination(pageIndex, pageSize, articleMap);
                articlePage.Page = (await article.ToListAsync()).MapToList<ArticleListView>();
                //填充文章的分页用户信息
                foreach (var page in articlePage.Page)
                {
                    //获取用户
                    var user = await userRpc.GetUserByIDAsync(new UserInfoRequest() { Id = page.UserId });
                    page.Nick = user.Nick;
                    page.Username = user.Username;
                }
                return articlePage;
            }
        }

        /// <summary>
        /// 发布文章
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<string> PublishArticle(PublishArticleReq request, int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var article = request.MapTo<Article>();
                article.UserId = uid;
                //获取全局文章设置
                var articleStatus = await dbContext.GlobalSettings.FirstOrDefaultAsync(g => g.Option == "ArticleStatus");
                var commentStatus = await dbContext.GlobalSettings.FirstOrDefaultAsync(g => g.Option == "CommentStatus");
                if (articleStatus != null && articleStatus.Value.HasValue)
                {
                    article.Status = articleStatus.Value.Value;
                }
                if (commentStatus != null && commentStatus.Value.HasValue)
                {
                    article.CommentStatus = commentStatus.Value.Value;
                }
                //保存修改
                dbContext.Articles.Add(article);
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("文章发布失败");
                }
                //添加文章标签
                articleTagApp.AddArticleTag(article.Id, request.Tags);
                //添加文章分类
                articleCategoryApp.AddArticleCategory(uid, article.Id, request.Categorys);
                return "发布成功";
            }
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> RemoveArticle(List<DelArticleReq> request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var articles = request.MapToList<Article>();
                dbContext.RemoveRange(articles);
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("删除失败");
                }
                return "删除成功";
            }
        }

        /// <summary>
        /// 删除文章(用户)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> RemoveArticle(List<DelArticleReq> request, int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询用户所有文章
                var articles = await dbContext.Articles.Where(a => a.UserId == uid).ToListAsync();
                foreach (var aid in request)
                {
                    var a = articles.FirstOrDefault(a => a.Id == aid.Id && a.UserId == uid);
                    if (a != null)
                    {
                        dbContext.Entry(a).State = EntityState.Deleted;
                    }
                }
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("删除失败");
                }
                return "删除成功";
            }
        }
    }
}
