using Infrastructure;
using UserService.App.Interface;
using UserService.Response;
using Microsoft.EntityFrameworkCore;
using UserService.Request;
using Microsoft.Extensions.Caching.Distributed;
using UserService.Domain;
using System.Text.RegularExpressions;

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
                    Nick = u.UserDetail.Nick,
                    Remark = u.UserDetail.Remark,
                    Photo = u.Photo,
                }).FirstOrDefaultAsync();
                var userView = user.MapTo<UserView>();
                return userView;
            }
        }


        /// <summary>
        /// 获取用户列表,分页+条件
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserView>> GetUsers(SearchCondition[] condition)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var users = context.Users.Select(u => new
                {
                    Nick = u.UserDetail.Nick,
                    Remark = u.UserDetail.Remark,
                    Photo = u.Photo,
                });
                //判断是否有条件
                if (condition.Length > 0)
                {
                    foreach (var con in condition)
                    {
                        users = "nick".Equals(con.Key,StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Nick.Contains(con.Value)) : users;
                        users = "remark".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Remark.Contains(con.Value)) : users;
                    }
                }
                var result = await users.ToListAsync();
                var usersView = result.MapToList<UserView>();
                return usersView;
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
                if (await dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username ||
                                                                   u.Phone == request.Phone ||
                                                                   u.Email == request.Email) != null)
                {
                    throw new Exception("用户已存在");
                }

                //验证验证码
                if ((request.Phone != null && await cache.GetStringAsync(request.Phone) == request.VerificationCode) ||
                    (request.Email != null && await cache.GetStringAsync(request.Email) == request.VerificationCode))
                {
                    //验证有效性
                    User user = request.MapTo<User>();
                    //密码加密
                    user.Password = user.Password.ShaEncrypt();
                    await dbContext.AddAsync(user);
                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        //查询用户ID
                        var u = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                        //添加用户详情
                        dbContext.UserDetails.Add(new UserDetail()
                        {
                            Nick = $"用户{user.Username}",
                            UserId = u.Id,
                            Sex = -1,
                            RegisterTime = DateTime.Now
                        });
                        await dbContext.SaveChangesAsync();
                        return "注册成功";
                    }
                    else
                    {
                        throw new Exception("注册失败");
                    }
                }
                else
                {
                    throw new Exception("验证码错误");
                }
            }
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="request">忘记密码请求</param>
        public async Task<string> ChangePassword(ForgetPasswordReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询用户
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
                if (user != null)
                {
                    //检查验证码
                    var phone = await cache.GetStringAsync(user?.Phone ?? "");
                    var email = await cache.GetStringAsync(user?.Email ?? "");
                    //验证有效性
                    if (phone == request.VerificationCode || email == request.VerificationCode)
                    {
                        user.Password = request.NewPassword.ShaEncrypt();
                        dbContext.Update(user);
                        if (await dbContext.SaveChangesAsync() > 0)
                        {
                            return "修改密码成功";
                        }
                        else
                        {
                            throw new Exception("修改密码失败");
                        }
                    }
                    else
                    {
                        throw new Exception("验证码错误");
                    }
                }
                else
                {
                    throw new Exception("用户信息错误");
                }
            }
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> ChangeUser(UpdateUserReq request, int id)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询用户
                var userDetail = await dbContext.UserDetails.FirstOrDefaultAsync(u => u.UserId == id);
                if (userDetail != null)
                {
                    userDetail.Nick = request.Nick ?? userDetail.Nick;
                    userDetail.Remark = request.Remark ?? userDetail.Remark;
                    userDetail.Sex = request.Sex ?? userDetail.Sex;
                    userDetail.Birthday = string.IsNullOrWhiteSpace(request.Birthday) ? Convert.ToDateTime(request.Birthday) : Convert.ToDateTime(userDetail.Birthday);
                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        return "更新成功";
                    }
                    else
                    {
                        throw new Exception("更新失败");
                    }
                }
                else
                {
                    throw new Exception("用户信息异常");
                }
            }
        }

        /// <summary>
        /// 账号绑定
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> BindAccount(BindAccountReq requests, int id)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Id == id && u.Password == requests.Password.ShaEncrypt());
                if (user != null)
                {
                    //查询验证码
                    if (await cache.GetStringAsync(requests.Bind) != requests.VerificationCode)
                    {
                        throw new Exception("验证码错误");
                    }
                    //判断绑定类型
                    if (requests.Type == "phone")
                    {
                        user.Phone = requests.Bind;
                    }
                    else if (requests.Type == "email")
                    {

                        user.Email = requests.Bind;
                    }
                    else
                    {
                        throw new Exception("绑定渠道错误:必须为phone、email");
                    }
                    //保存修改
                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        return "绑定成功";
                    }
                    else
                    {
                        throw new Exception("绑定失败");
                    }
                }
                else
                {
                    throw new Exception("用户信息错误");
                }
            }
        }

        /// <summary>
        /// 账号解绑
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> UbindAccount(UbindAccountReq requests, int id)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Id == id && u.Password == requests.Password.ShaEncrypt());
                if (user != null)
                {
                    if (requests.Type == "phone")
                    {
                        user.Phone = string.Empty;
                    }
                    else if (requests.Type == "email")
                    {
                        user.Email = string.Empty;
                    }
                    else
                    {
                        throw new Exception("绑定渠道错误:必须为phone、email");
                    }
                    //保存修改
                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        return "解绑成功";
                    }
                    else
                    {
                        throw new Exception("解绑成功");
                    }
                }
                else
                {
                    throw new Exception("用户信息错误");
                }
            }
        }
    }
}
