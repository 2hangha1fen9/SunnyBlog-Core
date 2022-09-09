using CommentService.App.Interface;
using CommentService.Domain;
using CommentService.Response;
using Google.Protobuf.WellKnownTypes;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using static ArticleService.Rpc.Protos.gArticle;
using static CommentService.Rpc.Protos.gUser;

namespace CommentService.App
{
    public class ViewApp : IViewApp
    {
        private readonly IDbContextFactory<CommentDBContext> contextFactory;
        private readonly gArticleClient articleRpc;
        private readonly gUserClient userRpc;

        public ViewApp(IDbContextFactory<CommentDBContext> contextFactory, gArticleClient articleRpc, gUserClient userRpc)
        {
            this.contextFactory = contextFactory;
            this.articleRpc = articleRpc;
            this.userRpc = userRpc;
        }



        /// <summary>
        /// 增加文章访问量
        /// </summary>
        /// <param name="aid">文章ID</param>
        /// <param name="uid">用户ID</param>
        /// <param name="ip">IP地址</param>
        public async Task AddArticleViewCount(int aid, int? uid = null,string? ip = null)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var view = new View()
                {
                    ArticleId = aid,
                    UserId = uid,
                    Ip = ip
                };
                dbContext.Views.Add(view);
                await dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 获取文章访问量
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        public async Task<int> GetArticleViewCount(int aid)
        {
            using(var dbContext = contextFactory.CreateDbContext())
            {
                var count = await dbContext.Views.CountAsync(v => v.ArticleId == aid);
                return count;
            }
        }

        /// <summary>
        /// 获取用户访问记录
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<PageList<UserViewHistory>> GetUserView(int uid, List<SearchCondition>? condition, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询所有用户信息
                var users = (await userRpc.GetUserListAsync(new Empty())).UserInfo;
                //查询所有文章
                var article = (await articleRpc.GetArticleListAsync(new Empty())).Infos;
                //查询所有访问记录
                var views = await dbContext.Views.ToListAsync();

                var history = (from v in views
                               join a in article on v.ArticleId equals a.Id
                               join u in users on v.UserId equals u.Id
                               where v.UserId == uid
                               select new UserViewHistory
                               {
                                   Id = v.Id,
                                   UserId = v.UserId,
                                   Username = u.Username,
                                   Nick = u.Nick,
                                   ArticleId = v.ArticleId,
                                   ArticleTitle = a.Title,
                                   Ip = v.Ip,
                                   ViewTime = v.ViewTime,
                               });

                //判断是否有条件
                if (condition?.Count > 0)
                {
                    foreach (var con in condition)
                    {

                        history = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? history.Where(u => u.Username != null && u.Username.Contains(con.Value)) : history;
                        history = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? history.Where(u => u.Nick != null && u.Nick.Contains(con.Value)) : history;
                        history = "ArticleTitle".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? history.Where(u => u.ArticleTitle.Contains(con.Value)) : history;
                    }
                }
                //对结果进行分页
                var historyPage = new PageList<UserViewHistory>();
                history = historyPage.Pagination(pageIndex, pageSize, history.AsQueryable()); //添加分页表表达式
                historyPage.Page = history.ToList(); //获取分页结果
                return historyPage;
            }
        }

        /// <summary>
        /// 获取所有访问记录
        /// </summary>
        /// <returns></returns>
        public async Task<PageList<UserViewHistory>> GetViewList(List<SearchCondition>? condition, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询所有用户信息
                var users = (await userRpc.GetUserListAsync(new Empty())).UserInfo;
                //查询所有文章
                var article = (await articleRpc.GetArticleListAsync(new Empty())).Infos;
                //查询所有访问记录
                var views = await dbContext.Views.ToListAsync();

                var history = (from v in views
                              join a in article on v.ArticleId equals a.Id
                              join u in users on v.UserId equals u.Id
                              select new UserViewHistory
                              {
                                  Id = v.Id,
                                  UserId = v.UserId,
                                  Username = u.Username,
                                  Nick = u.Nick,
                                  ArticleId = v.ArticleId,
                                  ArticleTitle = a.Title,
                                  Ip = v.Ip,
                                  ViewTime = v.ViewTime,
                              }).AsQueryable();

                //判断是否有条件
                if (condition?.Count > 0)
                {
                    foreach (var con in condition)
                    {
                        history = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? history.Where(u => u.Username != null && u.Username.Contains(con.Value)) : history;
                        history = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? history.Where(u => u.Nick != null && u.Nick.Contains(con.Value)) : history;
                        history = "ArticleTitle".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? history.Where(u => u.ArticleTitle.Contains(con.Value)) : history;
                    }
                }
                //对结果进行分页
                var historyPage = new PageList<UserViewHistory>();
                history = historyPage.Pagination(pageIndex, pageSize, history.AsQueryable()); //添加分页表表达式
                historyPage.Page = history.ToList(); //获取分页结果
                return historyPage;
            }
        }
    }
}
