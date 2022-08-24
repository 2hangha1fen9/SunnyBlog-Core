using Infrastructure;
using Microsoft.EntityFrameworkCore;
using UserService.App.Interface;
using UserService.Domain;
using UserService.Response;

namespace UserService.App
{
    public class FollowApp : IFollowApp
    {
        /// <summary>
        /// 数据库上下文工厂
        /// </summary>
        private readonly IDbContextFactory<UserDBContext> contextFactory;
        public FollowApp(IDbContextFactory<UserDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="sbId">被关注ID</param>
        /// <returns></returns>
        public async Task<string> CancelFollowSb(int id, int sbId)
        {
            try
            {
                using (var dbContext = contextFactory.CreateDbContext())
                {
                    var follow = await dbContext.UserFollows.FirstOrDefaultAsync(f => f.UserId == id && f.WatchId == sbId);
                    ;
                    if (follow != null)
                    {
                        dbContext.UserFollows.Remove(follow);
                        if (await dbContext.SaveChangesAsync() > 0)
                        {
                            return "取消成功";
                        }
                        else
                        {
                            throw new Exception("取消失败");
                        }
                    }
                    else
                    {
                        throw new Exception("还没有关注");
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("请求错误");
            }
            
        }

        /// <summary>
        /// 关注列表
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public async Task<PageList<FollowView>> FollowList(List<SearchCondition> condidtion, int id, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var follows = dbContext.UserFollows.Where(f => f.UserId == id).Select(f => new
                {
                    Id = f.UserId,
                    Nick = f.Watch.UserDetail.Nick,
                    Remark = f.Watch.UserDetail.Remark,
                    Photo = f.Watch.Photo
                });
                //筛选条件
                if (condidtion.Count > 0)
                {
                    foreach (var con in condidtion)
                    {
                        follows = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? follows.Where(f => f.Nick.Contains(con.Value)) : follows;
                        follows = "Remark".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? follows.Where(f => f.Remark.Contains(con.Value)) : follows;
                    }
                }
                //对结果进行分页
                var followsPage = new PageList<FollowView>();
                follows = followsPage.Pagination(pageIndex, pageSize, follows); //添加分页表表达式
                followsPage.Page = (await follows.ToListAsync()).MapToList<FollowView>(); //获取分页结果
                return followsPage;
            }
        }
        
        /// <summary>
        /// 关注某人
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="sbId">被关注用户ID</param>
        /// <returns></returns>
        public async Task<string> FollowSb(int id, int sbId)
        {
            try
            {
                using (var dbContext = contextFactory.CreateDbContext())
                {
                    if (id == sbId)
                    {
                        return "不能关注自己";
                    }
                    var follow = new UserFollow()
                    {
                        UserId = id,
                        WatchId = sbId,
                    };
                    await dbContext.AddAsync(follow);
                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        return "关注成功";
                    }
                    else
                    {
                        throw new Exception("关注失败");
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("请求错误");
            }
        }

        /// <summary>
        /// 关注状态
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="sbId">被关注用户ID</param>
        /// <returns></returns>
        public async Task<bool> FollowStatus(int id, int sbId)
        {
            try
            {
                using (var dbContext = contextFactory.CreateDbContext())
                {
                    var follow = await dbContext.UserFollows.FirstOrDefaultAsync(f => f.UserId == id && f.WatchId == sbId);
                    if (follow != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
