using Infrastructure;
using UserService.App.Interface;
using UserService.Response;
using Microsoft.EntityFrameworkCore;

namespace UserService.App
{
    public class UserApp : IUserApp
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly IDbContextFactory<UserDBContext> contextFactory;
        public UserApp(IDbContextFactory<UserDBContext> context)
        {
            this.contextFactory = context;
        }

        /// <summary>
        /// 根据ID查询用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns>用户视图</returns>
        public async Task<UserView> GetUserById(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var user = await context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
                var userView = user.MapTo<UserView>();
                return userView;
            }
        }

        public async Task<List<UserView>> GetUsers()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var users = await context.Users.ToListAsync();
                var usersView = users.MapToList<UserView>();
                return usersView;
            }
        }
    }
}
