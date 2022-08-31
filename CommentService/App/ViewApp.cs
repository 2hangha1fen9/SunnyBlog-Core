using CommentService.App.Interface;
using CommentService.Domain;
using CommentService.Response;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CommentService.App
{
    public class ViewApp : IViewApp
    {
        private readonly IDbContextFactory<CommentDBContext> contextFactory;
        public ViewApp(IDbContextFactory<CommentDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
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
        public async Task<List<UserViewHistory>> GetUserView(int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var uvh = await dbContext.Views.Where(v => v.UserId == uid).ToListAsync();
                return uvh.MapToList<UserViewHistory>();
            }
        }

        /// <summary>
        /// 获取所以访问记录
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserViewHistory>> GetViewList()
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var uvh = await dbContext.Views.ToListAsync();
                return uvh.MapToList<UserViewHistory>();
            }
        }
    }
}
