using Service.IdentityService.App.Interface;
using Service.IdentityService.Domain;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Service.IdentityService.App
{
    public class UserApp : IUserApp
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly AuthDBContext context;
        public UserApp(AuthDBContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 根据ID查询用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns>用户视图</returns>
        public User GetUser(string username,string password)
        {
            return context.Users.Where(u => u.Username == username && u.Password == password).FirstOrDefault();
        }
    }
}
