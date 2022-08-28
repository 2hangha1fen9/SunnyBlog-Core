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
        /// 编辑文章(管理)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> EditorArticle(EditorArticleReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询文章
                var article = await dbContext.Articles.FirstOrDefaultAsync(a => a.Id == request.Id);
                if (article != null)
                {
                    //文章更新
                    article.Title = request.Title ?? article.Title;
                    article.Content = request.Content ?? article.Content;
                    article.Photo = request.Photo ?? article.Photo;
                    article.RegionId = request.RegionId ?? article.RegionId;
                    article.Status = request.Status ?? article.Status;
                    article.CommentStatus = request.CommentStatus ?? article.CommentStatus;
                    article.UpdateTime = DateTime.Now;
                    article.IsLock = request.isLock ?? article.IsLock;
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
        /// 编辑文章(用户)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> EditorArticle(EditorArticleReq request, int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询文章
                var article = await dbContext.Articles.FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == uid && a.IsLock == 0);
                if (article != null)
                {
                    //文章更新
                    article.Title = request.Title ?? article.Title;
                    article.Content = request.Content ?? article.Content;
                    article.Photo = request.Photo ?? article.Photo;
                    article.RegionId = request.RegionId ?? article.RegionId;
                    article.Status = request.Status ?? article.Status;
                    article.CommentStatus = request.CommentStatus ?? article.CommentStatus;
                    article.UpdateTime = DateTime.Now;
                    //标签更新
                    if (request.Tags?.Count > 0)
                    {
                        articleTagApp.UpdateArticleTag(article.Id, request.Tags);
                    }
                    //分区更新
                    if (request.Categorys?.Count > 0)
                    {
                        articleCategoryApp.UpdateArticleCategory(uid, article.Id, request.Categorys);
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
                    UserId = a.UserId,
                    Title = a.Title,
                    Content = a.Content,
                    Photo = a.Photo,
                    Tags = a.ArticleTags.Where(at => at.Tag.IsPrivate == 1).Select(at => new TagView()
                    {
                        Id = at.Id,
                        UserId = at.Tag.UserId,
                        Name = at.Tag.Name,
                        Color = at.Tag.Color
                    }),
                    RegionName = a.Region.Name,
                    RegionId = a.RegionId,
                    CreateTime = a.CreateTime,
                    Status = a.Status,
                    CommentStatus = a.CommentStatus,
                    IsLock = a.IsLock
                }).FirstOrDefaultAsync(a => a.Id == id && a.Status == 1);
                if (article != null)
                {
                    var adv = article.MapTo<ArticleView>();
                    //调用rgc方法获取用户信息
                    var userInfo = userRpc.GetUserByID(new UserInfoRequest() { Id = article.UserId });
                    if (userInfo != null)
                    {
                        adv.Nick = userInfo.Nick;
                        adv.UserPhoto = userInfo.Photo;
                    }
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
        public async Task<PageList<ArticleView>> GetArticleList(List<SearchCondition> condidtion, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //创建数据映射
                var article = dbContext.Articles.Select(a => new
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UserPhoto = "",
                    Nick = "",
                    Title = a.Title,
                    Content = a.Content.Substring(0, 100),
                    Photo = a.Photo,
                    Tags = a.ArticleTags.Where(at => at.Tag.IsPrivate == 1).Select(at => new TagView()
                    {
                        Id = at.Id,
                        UserId = at.Tag.UserId,
                        Name = at.Tag.Name,
                        Color = at.Tag.Color
                    }),
                    RegionName = a.Region.Name,
                    RegionId = a.RegionId,
                    CreateTime = a.CreateTime,
                    Status = a.Status,
                    CommentStatus = a.CommentStatus,
                    IsLock = a.IsLock
                }).Where(a => a.Status == 1);
                //筛选条件
                if (condidtion.Count > 0)
                {
                    foreach (var con in condidtion)
                    {
                        article = "Title".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.Title.Contains(con.Value)) : article;
                        article = "Content".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.Content.Contains(con.Value)) : article;
                        article = "Tag".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.Tags.Where(t => t.Name.Contains(con.Value) || t.Id == Convert.ToInt32(con.Value)).Count() > 0) : article;
                        article = "Region".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.RegionName.Contains(con.Value) || a.RegionId == Convert.ToInt32(con.Value)) : article;
                    }
                }
                //分页条件
                var articlePage = new PageList<ArticleView>();
                article = articlePage.Pagination(pageIndex, pageSize, article);
                articlePage.Page = (await article.ToListAsync()).MapToList<ArticleView>();
                //填充文章的分页用户信息
                foreach (var page in articlePage.Page)
                {
                    //获取用户
                    var user = await userRpc.GetUserByIDAsync(new UserInfoRequest() { Id = page.UserId });
                    page.Nick = user.Nick;
                    page.UserPhoto = user.Photo;
                }
                return articlePage;
            }
        }
        
        /// <summary>
        /// 用户文章列表
        /// </summary>
        public async Task<PageList<ArticleView>> GetUserArticleList(List<SearchCondition> condidtion,int uid, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //创建数据映射
                var article = dbContext.Articles.Select(a => new
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UserPhoto = "",
                    Nick = "",
                    Title = a.Title,
                    Content = a.Content.Substring(0, 100),
                    Photo = a.Photo,
                    Tags = a.ArticleTags.Where(at => at.Tag.IsPrivate == 1).Select(at => new TagView()
                    {
                        Id = at.Id,
                        UserId = at.Tag.UserId,
                        Name = at.Tag.Name,
                        Color = at.Tag.Color
                    }),
                    RegionName = a.Region.Name,
                    RegionId = a.RegionId,
                    CreateTime = a.CreateTime,
                    Status = a.Status,
                    CommentStatus = a.CommentStatus,
                    IsLock = a.IsLock
                }).Where(a => a.Status == 1 && a.UserId == uid);
                //筛选条件
                if (condidtion.Count > 0)
                {
                    foreach (var con in condidtion)
                    {
                        article = "Title".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.Title.Contains(con.Value)) : article;
                        article = "Content".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.Content.Contains(con.Value)) : article;
                        article = "Tag".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.Tags.Where(t => t.Name.Contains(con.Value) || t.Id == Convert.ToInt32(con.Value)).Count() > 0) : article;
                        article = "Region".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.RegionName.Contains(con.Value) || a.RegionId == Convert.ToInt32(con.Value)) : article;
                    }
                }
                //分页条件
                var articlePage = new PageList<ArticleView>();
                article = articlePage.Pagination(pageIndex, pageSize, article);
                articlePage.Page = (await article.ToListAsync()).MapToList<ArticleView>();
                //填充文章的分页用户信息
                foreach (var page in articlePage.Page)
                {
                    //获取用户
                    var user = await userRpc.GetUserByIDAsync(new UserInfoRequest() { Id = page.UserId });
                    page.Nick = user.Nick;
                    page.UserPhoto = user.Photo;
                }
                return articlePage;
            }
        }

        /// <summary>
        /// 获取文章行列表
        /// </summary>
        /// <param name="condidtion"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PageList<ArticleListView>> GetRowList(List<SearchCondition> condidtion, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var article = dbContext.Articles.Select(a => new
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    Title = a.Title,
                    RegionName = a.Region.Name,
                    RegionId = a.RegionId,
                    Username = "",
                    Status = a.Status,
                    CommentStatus = a.CommentStatus,
                    CreateTime = a.CreateTime,
                    IsLock = a.IsLock
                });
                //筛选条件
                if (condidtion.Count > 0)
                {
                    foreach (var con in condidtion)
                    {
                        article = "Title".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.Title.Contains(con.Value)) : article;
                        article = "Region".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.RegionName.Contains(con.Value) || a.RegionId == Convert.ToInt32(con.Value)) : article;
                        article = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.Username.Contains(con.Value)) : article;
                        article = "Status".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.Status == Convert.ToInt32(con.Value)) : article;
                        article = "IsLock".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.IsLock == Convert.ToInt32(con.Value)) : article;
                        article = "CommentStatus".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.CommentStatus == Convert.ToInt32(con.Value)) : article;
                    }
                }
                //分页条件
                var articlePage = new PageList<ArticleListView>();
                article = articlePage.Pagination(pageIndex, pageSize, article);
                articlePage.Page = (await article.ToListAsync()).MapToList<ArticleListView>();
                //填充文章的分页用户信息
                foreach (var page in articlePage.Page)
                {
                    //获取用户
                    var user = await userRpc.GetUserByIDAsync(new UserInfoRequest() { Id = page.UserId });
                    page.Username = user.Username;
                }
                return articlePage;
            }
        }

        /// <summary>
        /// 获取文章行列表(用户)
        /// </summary>
        /// <param name="condidtion"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PageList<ArticleListView>> GetRowList(List<SearchCondition> condidtion, int uid, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var article = dbContext.Articles.Where(a => a.UserId == uid).Select(a => new
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    Title = a.Title,
                    RegionName = a.Region.Name,
                    RegionId = a.RegionId,
                    Category = a.ArtCategories.Select(c => new CategoryView()
                    {
                        Id = c.Id,
                        Name = c.Category.Name,
                        UserId = c.Category.UserId,
                        ParentId = c.Category.ParentId ?? 0,
                        InverseParent = null
                    }),
                    Username = "",
                    Status = a.Status,
                    CommentStatus = a.CommentStatus,
                    CreateTime = a.CreateTime,
                    IsLock = a.IsLock
                });
                //筛选条件
                if (condidtion.Count > 0)
                {
                    foreach (var con in condidtion)
                    {
                        article = "Title".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.Title.Contains(con.Value)) : article;
                        article = "Region".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.RegionName.Contains(con.Value) || a.RegionId == Convert.ToInt32(con.Value)) : article;
                        article = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.Username.Contains(con.Value)) : article;
                        article = "Status".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.Status == Convert.ToInt32(con.Value)) : article;
                        article = "IsLock".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.IsLock == Convert.ToInt32(con.Value)) : article;
                        article = "CommentStatus".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.CommentStatus == Convert.ToInt32(con.Value)) : article;
                        article = "Category".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? article.Where(a => a.Category.Where(c => c.Name.Contains(con.Value) || c.Id == Convert.ToInt32(con.Value)).Count() > 0) : article;
                    }
                }
                //分页条件
                var articlePage = new PageList<ArticleListView>();
                article = articlePage.Pagination(pageIndex, pageSize, article);
                articlePage.Page = (await article.ToListAsync()).MapToList<ArticleListView>();
                //填充文章的分页用户信息
                foreach (var page in articlePage.Page)
                {
                    //获取用户
                    var user = await userRpc.GetUserByIDAsync(new UserInfoRequest() { Id = page.UserId });
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
