using Infrastructure;
using UserService.App.Interface;
using UserService.Response;
using Microsoft.EntityFrameworkCore;
using UserService.Request;
using Microsoft.Extensions.Caching.Distributed;
using UserService.Domain;

namespace UserService.App
{
    public class UserApp : IUserApp
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly IDbContextFactory<UserDBContext> contextFactory;
        /// <summary>
        /// Redis缓存客户端
        /// </summary>
        private readonly IDistributedCache cache;
        public UserApp(IDbContextFactory<UserDBContext> context, IDistributedCache cache)
        {
            this.contextFactory = context;
            this.cache = cache;
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
                var user = await context.Users.Where(u => u.Id == id).Select(u => new
                {
                    Id = u.Id,
                    Username = u.Username,
                    Nick = u.UserDetail.Nick,
                    Email = u.Email,
                    Photo = u.Photo,
                }).FirstOrDefaultAsync();
                var userView = user.MapTo<UserView>();
                return userView;
            }
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserView>> GetUsers()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var users = await context.Users.Select(u => new
                {
                    Id = u.Id,
                    Username = u.Username,
                    Nick = u.UserDetail.Nick,
                    Email = u.Email,
                    Photo = u.Photo,
                }).ToListAsync();
                var usersView = users.MapToList<UserView>();
                return usersView;
            }
        }

        /// <summary>
        /// 获取用户详细列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserDetailView>> GetUserDetails()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var users = await context.UserDetails.Select(u => new
                {
                    UserId = u.UserId,
                    Username = u.User.Username,
                    Nick = u.Nick,
                    Phone = u.User.Phone,
                    Email = u.User.Email,
                    Sex = u.Sex,
                    Birthday = u.Birthday,
                    RegisterTime = u.RegisterTime,
                    Remark = u.Remark,
                    Score = u.Score,
                    Photo = u.User.Photo,
                    Status = u.User.Status
                }).ToListAsync();
                var usersView = users.MapToList<UserDetailView>();
                return usersView;
            }
        }
        
        /// <summary>
        /// 根据用户ID获取用户详情
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public async Task<UserDetailView> GetUserDetailById(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var user = await context.UserDetails.Where(u => u.UserId == u.UserId).Select(u => new
                {
                    UserId = u.UserId,
                    Username = u.User.Username,
                    Nick = u.Nick,
                    Phone = u.User.Phone,
                    Email = u.User.Email,
                    Sex = u.Sex,
                    Birthday = u.Birthday,
                    RegisterTime = u.RegisterTime,
                    Remark = u.Remark,
                    Score = u.Score,
                    Photo = u.User.Photo,
                    Status = u.User.Status
                }).FirstOrDefaultAsync();
                var userView = user.MapTo<UserDetailView>();
                return userView;
            }
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> UserRegister(UserRegisterReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询用户是否存在
                if(await dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Phone == request.Phone) != null)
                {
                    return "用户已存在";
                }

                if (cache.GetString(request.Phone) == request.VerificationCode)
                {
                    if (dbContext.Add(request.MapTo<User>()) != null)
                    {
                        return "注册成功";
                    }
                    else
                    {
                        return "注册失败";
                    }
                }
                else
                {
                    return "验证码错误";
                }
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<string> SendMessage(string phone)
        {
            throw new NotImplementedException();
        }
    }
}
