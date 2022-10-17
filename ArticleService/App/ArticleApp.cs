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
using static CommentService.Rpc.Protos.gRank;
using CommentService.Rpc.Protos;
using Google.Protobuf.WellKnownTypes;
using static CommentService.Rpc.Protos.gMark;

namespace ArticleService.App
{
    public class ArticleApp : IArticleApp
    {
        private readonly IDbContextFactory<ArticleDBContext> contextFactory;
        private readonly IArticleTagApp articleTagApp;
        private readonly IArticleRegionApp articleRegionApp;
        private readonly gUserClient userRpc;
        private readonly gRankClient rankRpc;
        private readonly gMarkClient markRpc;

        public ArticleApp(IDbContextFactory<ArticleDBContext> contextFactory, IArticleTagApp articleTagApp, gUserClient userRpc, IArticleRegionApp articleRegionApp, gRankClient rankRpc, gMarkClient markRpc)
        {
            this.contextFactory = contextFactory;
            this.articleTagApp = articleTagApp;
            this.userRpc = userRpc;
            this.articleRegionApp = articleRegionApp;
            this.rankRpc = rankRpc;
            this.markRpc = markRpc;
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
                    article = await dbContext.Articles.FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == uid && a.IsLock == 1);
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
                    article.CodeStyle = request.CodeStyle ?? article.CodeStyle;
                    article.ContentStyle = request.ContentStyle ?? article.ContentStyle;
                    article.RegionId = request.RegionId.HasValue && request.RegionId.Value != 0 ? request.RegionId.Value : null;
                    article.CategoryId = request.CategoryId.HasValue && request.CategoryId.Value != 0 ? request.CategoryId.Value : null;
                    article.Status = request.Status ?? article.Status;
                    article.IsLock = request.isLock ?? article.IsLock;
                    article.CommentStatus = request.CommentStatus ?? article.CommentStatus;
                    article.UpdateTime = DateTime.Now;
                    //标签更新
                    if (request.Tags?.Count > 0)
                    {
                        articleTagApp.UpdateArticleTag(article.Id, request.Tags);
                    }
                    //保存修改
                    dbContext.Entry(article).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                    return "更新成功";
                }
                else
                {
                    throw new Exception("更新失败");
                }
            }
        }

        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ArticleView> GetArticle(int id,bool allScope = false)
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
                    CodeStyle = a.CodeStyle,
                    ContentStyle = a.ContentStyle,
                    Summary = a.Summary,
                    Photo = a.Photo,
                    RegionName = a.Region.Name,
                    RegionId = a.RegionId,
                    Tags = a.ArticleTags.Select(at => new TagView()
                    {
                        Id = at.TagId,
                        UserId = at.Tag.UserId,
                        Name = at.Tag.Name,
                        Color = at.Tag.Color,
                        ArticleCount = at.Tag.ArticleTags.Count(),
                    }),
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category.Name,
                    Status = a.Status,
                    CommentStatus = a.CommentStatus,
                    CreateTime = a.CreateTime,
                    UpdateTime = a.UpdateTime
                }).FirstOrDefaultAsync(a => a.Id == id && (allScope || a.Status == 1));

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
        public async Task<PageList<ArticleListView>> GetArticleList(List<SearchCondition>? condidtion, Expression<Func<Article, bool>> predict , int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //创建数据映射
                var articleQuery = dbContext.Articles.Include(a => a.ArticleTags)
                                                    .ThenInclude(at => at.Tag)
                                                    .Include(a => a.Category)
                                                    .Include(a => a.Region).AsQueryable();
                //使用前置条件,判断是否是只需要用户的文章,还是用户自己个人的文章,还是所有文章
                if (predict != null)
                {
                    articleQuery = articleQuery.Where(predict);
                }
                //查询所有文章
                var articleList = await articleQuery.ToListAsync();
                var userList = (await userRpc.GetUserListAsync(new Empty())).UserInfo.ToList();
                var articleMap = articleList.Join(userList,a => a.UserId,u => u.Id,(a,u) => new
                { 
                    Id = a.Id,
                    UserId = a.UserId,
                    Nick = u.Nick,
                    Username = u.Username,
                    Title = a.Title,
                    Summary = a.Summary,
                    Photo = a.Photo,
                    RegionName = a.Region?.Name,
                    RegionId = a.RegionId,
                    Tags = a.ArticleTags.Select(at => new TagView()
                    {
                        Id = at.TagId,
                        UserId = at.Tag.UserId,
                        Name = at.Tag.Name,
                        Color = at.Tag.Color,
                        ArticleCount = at.Tag.ArticleTags.Count(),
                    }),
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category?.Name,
                    Status = a.Status,
                    IsLock = a.IsLock,
                    CommentStatus = a.CommentStatus,
                    CreateTime = a.CreateTime,
                    UpdateTime = a.UpdateTime,
                });
                //筛选条件
                if (condidtion?.Count > 0)
                {
                    foreach (var con in condidtion)
                    {
                        //条件过滤
                        articleMap = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Username.Contains(con.Value,StringComparison.OrdinalIgnoreCase)) : articleMap;
                        articleMap = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Nick.Contains(con.Value,StringComparison.OrdinalIgnoreCase)) : articleMap;
                        articleMap = "Title".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Title.Contains(con.Value,StringComparison.OrdinalIgnoreCase)) : articleMap;
                        articleMap = "Summary".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Summary != null && a.Summary.Contains(con.Value,StringComparison.OrdinalIgnoreCase)) : articleMap;
                        articleMap = "Tag".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Tags.Where(t => t.Name == con.Value).Count() > 0) : articleMap;
                        articleMap = "Category".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.CategoryName != null && a.CategoryName.Contains(con.Value,StringComparison.OrdinalIgnoreCase)): articleMap;
                        articleMap = "Status".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Status == Convert.ToInt32(con.Value)) : articleMap;
                        articleMap = "IsLock".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.IsLock == Convert.ToInt32(con.Value)) : articleMap;
                        articleMap = "CommentStatus".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.CommentStatus == Convert.ToInt32(con.Value)) : articleMap;
                        if ("Article".Equals(con.Key, StringComparison.OrdinalIgnoreCase) && con.Value != null)
                        {
                            articleMap = articleMap.Where(a => a.Title.Contains(con.Value, StringComparison.OrdinalIgnoreCase) || (a.Summary != null && a.Summary.Contains(con.Value, StringComparison.OrdinalIgnoreCase)));
                        }
                        if ("CategoryId".Equals(con.Key, StringComparison.OrdinalIgnoreCase) && con.Value != null)
                        {
                            articleMap = articleMap.Where(a => a.CategoryId == Convert.ToInt32(con.Value));
                        }
                        else if ("CategoryId".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            articleMap = articleMap.Where(a => a.CategoryId == null);
                        }
                        //分区过滤
                        if ("Region".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            //获取所有与之相关的分区
                            var filterRegion = await articleRegionApp.GetRegions(con.Value);
                            //和离线数据查询需要将结果输出
                            var tempArticleMap = articleMap.ToList();
                            articleMap = tempArticleMap.Where(a => filterRegion.FirstOrDefault(r => r.Id == a.RegionId) != null).AsQueryable();
                        }
                        //排序
                        if (con.Sort != 0)
                        {
                            //排行榜排序
                            if ((new string[] { "Hot","ViewCount", "LikeCount", "CommentCount", "CollectionCount" }).FirstOrDefault(c => c.Equals(con.Key, StringComparison.OrdinalIgnoreCase)) != null)
                            {
                                //和离线数据查询需要将结果输出
                                var tempArticleMap = articleMap.ToList();
                                var rank = (await rankRpc.GetArticleRankAsync(new RankCondidtion
                                {
                                    Key = con.Key,
                                    Order = con.Sort ?? 1
                                })).Ranks.Select(r => r).ToArray();
                                articleMap = tempArticleMap.OrderBy(a => Array.IndexOf(rank, a.Id)).AsQueryable();
                            }
                            if ("CreateTime".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                            {
                                if(con.Sort == -1)
                                {
                                    articleMap = articleMap.OrderByDescending(a => a.CreateTime);
                                }
                                else
                                {
                                    articleMap = articleMap.OrderBy(a => a.CreateTime);
                                }
                            }
                            if ("UpdateTime".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                            {
                                if (con.Sort == -1)
                                {
                                    articleMap = articleMap.OrderByDescending(a => a.UpdateTime);
                                }
                                else
                                {
                                    articleMap = articleMap.OrderBy(a => a.UpdateTime);
                                }
                            }
                        }
                        else
                        {
                            //默认时间排序
                            articleMap = articleMap.OrderByDescending(a => a.CreateTime);
                        }
                    }
                }
                else
                {
                    //默认时间排序
                    articleMap = articleMap.OrderByDescending(a => a.CreateTime);
                }
                //分页条件
                var articlePage = new PageList<ArticleListView>();
                articleMap = articlePage.Pagination(pageIndex, pageSize, articleMap.AsQueryable());
                articlePage.Page = articleMap.MapToList<ArticleListView>();
                return articlePage;
            }
        }

        /// <summary>
        /// 获取用户的点赞/收藏文章
        /// </summary>
        /// <param name="condidtion"></param>
        /// <param name="predict"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PageList<ArticleListView>> GetUserLikeArticle(int uid,bool isLike,List<SearchCondition>? condidtion, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //创建数据映射
                var articleQuery = dbContext.Articles.Include(a => a.ArticleTags)
                                                    .ThenInclude(at => at.Tag)
                                                    .Include(a => a.Category)
                                                    .Include(a => a.Region).AsQueryable();
                //查询所有文章
                var articleList = await articleQuery.ToListAsync();
                var userList = (await userRpc.GetUserListAsync(new Empty())).UserInfo.ToList();
                //获取用户点赞的文章id
                var likeIds = (await markRpc.GetUserLikeAsync(new RequestInfo { Uid = uid,Type = isLike ? 1 : 0})).Ids.ToList();
                articleList = articleList.Where(a => likeIds.IndexOf(a.Id) >= 0).ToList();
                var articleMap = articleList.Join(userList, a => a.UserId, u => u.Id, (a, u) => new
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    Nick = u.Nick,
                    Username = u.Username,
                    Title = a.Title,
                    Summary = a.Summary,
                    Photo = a.Photo,
                    RegionName = a.Region?.Name,
                    RegionId = a.RegionId,
                    Tags = a.ArticleTags.Select(at => new TagView()
                    {
                        Id = at.TagId,
                        UserId = at.Tag.UserId,
                        Name = at.Tag.Name,
                        Color = at.Tag.Color,
                        ArticleCount = at.Tag.ArticleTags.Count(),
                    }),
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category?.Name,
                    Status = a.Status,
                    IsLock = a.IsLock,
                    CommentStatus = a.CommentStatus,
                    CreateTime = a.CreateTime,
                    UpdateTime = a.UpdateTime,
                });
                //筛选条件
                if (condidtion?.Count > 0)
                {
                    foreach (var con in condidtion)
                    {
                        //条件过滤
                        articleMap = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Username.Contains(con.Value, StringComparison.OrdinalIgnoreCase)) : articleMap;
                        articleMap = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Nick.Contains(con.Value, StringComparison.OrdinalIgnoreCase)) : articleMap;
                        articleMap = "Title".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Title.Contains(con.Value, StringComparison.OrdinalIgnoreCase)) : articleMap;
                        articleMap = "Summary".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Summary != null && a.Summary.Contains(con.Value, StringComparison.OrdinalIgnoreCase)) : articleMap;
                        articleMap = "Tag".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Tags.Where(t => t.Name == con.Value).Count() > 0) : articleMap;
                        articleMap = "Category".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.CategoryName != null && a.CategoryName.Contains(con.Value, StringComparison.OrdinalIgnoreCase)) : articleMap;
                        articleMap = "Status".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.Status == Convert.ToInt32(con.Value)) : articleMap;
                        articleMap = "IsLock".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.IsLock == Convert.ToInt32(con.Value)) : articleMap;
                        articleMap = "CommentStatus".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? articleMap.Where(a => a.CommentStatus == Convert.ToInt32(con.Value)) : articleMap;
                        if ("Article".Equals(con.Key, StringComparison.OrdinalIgnoreCase) && con.Value != null)
                        {
                            articleMap = articleMap.Where(a => a.Title.Contains(con.Value, StringComparison.OrdinalIgnoreCase) || (a.Summary != null && a.Summary.Contains(con.Value, StringComparison.OrdinalIgnoreCase)));
                        }
                        if ("CategoryId".Equals(con.Key, StringComparison.OrdinalIgnoreCase) && con.Value != null)
                        {
                            articleMap = articleMap.Where(a => a.CategoryId == Convert.ToInt32(con.Value));
                        }
                        else if ("CategoryId".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            articleMap = articleMap.Where(a => a.CategoryId == null);
                        }
                        //分区过滤
                        if ("Region".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            //获取所有与之相关的分区
                            var filterRegion = await articleRegionApp.GetRegions(con.Value);
                            //和离线数据查询需要将结果输出
                            var tempArticleMap = articleMap.ToList();
                            articleMap = tempArticleMap.Where(a => filterRegion.FirstOrDefault(r => r.Id == a.RegionId) != null).AsQueryable();
                        }
                        //排序
                        if (con.Sort != 0)
                        {
                            //排行榜排序
                            if ((new string[] { "Hot", "ViewCount", "LikeCount", "CommentCount", "CollectionCount" }).FirstOrDefault(c => c.Equals(con.Key, StringComparison.OrdinalIgnoreCase)) != null)
                            {
                                //和离线数据查询需要将结果输出
                                var tempArticleMap = articleMap.ToList();
                                var rank = (await rankRpc.GetArticleRankAsync(new RankCondidtion
                                {
                                    Key = con.Key,
                                    Order = con.Sort ?? 1
                                })).Ranks.Select(r => r).ToArray();
                                articleMap = tempArticleMap.OrderBy(a => Array.IndexOf(rank, a.Id)).AsQueryable();
                            }
                            if ("CreateTime".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                            {
                                if (con.Sort == -1)
                                {
                                    articleMap = articleMap.OrderByDescending(a => a.CreateTime);
                                }
                                else
                                {
                                    articleMap = articleMap.OrderBy(a => a.CreateTime);
                                }
                            }
                            if ("UpdateTime".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                            {
                                if (con.Sort == -1)
                                {
                                    articleMap = articleMap.OrderByDescending(a => a.UpdateTime);
                                }
                                else
                                {
                                    articleMap = articleMap.OrderBy(a => a.UpdateTime);
                                }
                            }
                        }
                        else
                        {
                            //默认时间排序
                            articleMap = articleMap.OrderByDescending(a => a.CreateTime);
                        }
                    }
                }
                else
                {
                    //默认时间排序
                    articleMap = articleMap.OrderByDescending(a => a.CreateTime);
                }
                //分页条件
                var articlePage = new PageList<ArticleListView>();
                articleMap = articlePage.Pagination(pageIndex, pageSize, articleMap.AsQueryable());
                articlePage.Page = articleMap.MapToList<ArticleListView>();
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
                if (articleStatus != null && articleStatus.Value.HasValue && articleStatus.Value != 1)
                {
                    article.Status = articleStatus.Value.Value;
                    if(article.Status == -2)
                    {
                        throw new Exception("禁止发布文章");
                    }
                }
                if (commentStatus != null && commentStatus.Value.HasValue && commentStatus.Value != 1)
                {
                    article.CommentStatus = commentStatus.Value.Value;
                }
                article.UpdateTime = DateTime.Now;
                //保存修改
                dbContext.Articles.Add(article);
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("文章发布失败");
                }
                //添加文章标签
                articleTagApp.AddArticleTag(article.Id, request.Tags);
                return $"{article.Id}";
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
        /// 删除文章(需要用户ID)
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
                        if (a.Status == 3)
                        {
                            //如果已经在回收站则直接删除
                            dbContext.Entry(a).State = EntityState.Deleted;
                        }
                        else
                        {
                            //第一次先进行逻辑删除
                            a.Status = 3;
                            dbContext.Entry(a).State = EntityState.Modified;
                        }
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
