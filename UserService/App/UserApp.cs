using Infrastructure;
using UserService.App.Interface;
using UserService.Response;

namespace UserService.App
{
    public class UserApp : IUserApp
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly UserDBContext context;
        public UserApp(UserDBContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 根据ID查询用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns>用户视图</returns>
        public UserView GetUserById(int id)
        {
            var user = context.Users.Where(u => u.Id == id).FirstOrDefault();
            var userView = user.MapTo<UserView>();
            return userView;
        }

        public List<UserView> GetUsers()
        {
            var users = context.Users.ToList();
            var usersView = users.MapToList<UserView>();
            return usersView;
        }
    }
}
