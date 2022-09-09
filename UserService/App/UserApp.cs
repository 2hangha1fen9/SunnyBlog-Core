using Infrastructure;
using UserService.App.Interface;
using UserService.Response;
using Microsoft.EntityFrameworkCore;
using UserService.Request;
using Microsoft.Extensions.Caching.Distributed;
using UserService.Domain;
using UserService.Request.Request;
using System.Linq;
using StackExchange.Redis;

namespace UserService.App
{
    public class UserApp : IUserApp
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly IDbContextFactory<UserDBContext> contextFactory;
        /// <summary>
        /// Redis客户端,依赖注入
        /// </summary>
        private readonly IDatabase database;

        public UserApp(IDbContextFactory<UserDBContext> context, IConnectionMultiplexer connection)
        {
            this.contextFactory = context;
            this.database = connection.GetDatabase();
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
        public async Task<PageList<UserView>> GetUsers(List<SearchCondition>? condition, int pageIndex, int pageSize)
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
                if (condition?.Count > 0)
                {
                    foreach (var con in condition)
                    {
                        users = "nick".Equals(con.Key,StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Nick.Contains(con.Value)) : users;
                        users = "remark".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Remark.Contains(con.Value)) : users;
                    }
                }
                //对结果进行分页
                var userPage = new PageList<UserView>();
                users = userPage.Pagination(pageIndex,pageSize,users); //添加分页表表达式
                userPage.Page = (await users.ToListAsync()).MapToList<UserView>(); //获取分页结果
                return userPage;
            }
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public async Task<PageList<UserDetailView>> GetUserDetails(List<SearchCondition>? condition, int pageIndex, int pageSize)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var users = context.Users.Select(u => new
                {
                    Id = u.Id,
                    Username = u.Username,
                    Nick = u.UserDetail.Nick,
                    Phone = u.Phone,
                    Email = u.Email,
                    Sex = u.UserDetail.Sex,
                    Birthday = u.UserDetail.Birthday,
                    RegisterTime = u.UserDetail.RegisterTime,
                    Remark = u.UserDetail.Remark,
                    Photo = u.Photo,
                    Score = u.UserDetail == null ? 0.0m : u.UserDetail.Score,
                    Status = u.Status
                });
                //判断是否有条件
                if (condition?.Count > 0)
                {
                    foreach (var con in condition)
                    {
                        users = "Id".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Id == Convert.ToInt32(con.Value)) : users;
                        users = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Username.Contains(con.Value)) : users;
                        users = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Nick.Contains(con.Value)) : users;
                        users = "Phone".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Phone.Contains(con.Value)) : users;
                        users = "Email".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Email.Contains(con.Value)) : users;
                        users = "Sex".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Sex == Convert.ToInt32(con.Value)) : users;
                        users = "Remark".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Remark.Contains(con.Value)) : users;
                        users = "Status".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Status == Convert.ToInt32(con.Value)) : users;
                    }
                }
                //对结果进行分页
                var userPage = new PageList<UserDetailView>();
                users = userPage.Pagination(pageIndex, pageSize, users); //添加分页表表达式
                userPage.Page = (await users.ToListAsync()).MapToList<UserDetailView>(); //获取分页结果
                return userPage;
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
                var user = await context.Users.Select(u => new
                {
                    Id = u.Id,
                    Username = u.Username,
                    Nick = u.UserDetail.Nick,
                    Phone = u.Phone,
                    Email = u.Email,
                    Sex = u.UserDetail.Sex,
                    Birthday = u.UserDetail.Birthday,
                    RegisterTime = u.UserDetail.RegisterTime,
                    Remark = u.UserDetail.Remark,
                    Photo = u.Photo,
                    Score = u.UserDetail == null ? 0.0m : u.UserDetail.Score,
                    Status = u.Status
                }).FirstOrDefaultAsync(u => u.Id == id);
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
                if (await dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username ||
                                                                   (u.Phone ?? "-1") == request.Phone ||
                                                                   (u.Email ?? "-1") == request.Email) != null)
                {
                    throw new Exception("用户已存在");
                }

                //验证验证码
                if ((await database.StringGetDeleteAsync($"VCode:{request.Phone}") == request.VerificationCode) ||
                    (await database.StringGetDeleteAsync($"VCode:{request.Email}") == request.VerificationCode))
                {
                    //验证有效性
                    User user = request.MapTo<User>();
                    //密码加密
                    user.Password = user.Password.ShaEncrypt();
                    await dbContext.AddAsync(user);
                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        //添加用户详情
                        dbContext.UserDetails.Add(new UserDetail()
                        {
                            Nick = $"用户{user.Username}",
                            UserId = user.Id,
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
                    var phone = await database.StringGetDeleteAsync($"VCode:{user.Phone}");
                    var email = await database.StringGetDeleteAsync($"VCode:{user.Email}");
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
        public async Task<string> ChangeUser(ChangeUserReq request, int id)
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
                    dbContext.Entry(userDetail).State = EntityState.Modified;
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
                    if (await database.StringGetDeleteAsync($"VCode:{requests.Bind}") != requests.VerificationCode)
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
                    dbContext.Entry(user).State = EntityState.Modified;
                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        //移除键
                        await database.KeyDeleteAsync($"VCode:{requests.Bind}");
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
                    dbContext.Entry(user).State = EntityState.Modified;
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

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> AddUser(AddUserReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询用户是否存在
                if (await dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username ||
                                                                   (u.Phone ?? "-1") == request.Phone ||
                                                                   (u.Email ?? "-1") == request.Email) != null)
                {
                    throw new Exception("用户已存在");
                }
                var user = new User()
                {
                    Username = request.Username,
                    Password = request.Password.ShaEncrypt(),
                    Phone = request.Phone,
                    Email = request.Email,
                    Status = request.Status ?? 0
                };
                await dbContext.Users.AddAsync(user);
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("添加失败");
                }

                var userDetail = new UserDetail()
                {
                    UserId = user.Id,
                    Nick = request.Nick,
                    Sex = request.Sex,
                    Birthday = Convert.ToDateTime(request.Birthday),
                    RegisterTime = DateTime.Now,
                    Remark = request.Remark,
                    Score = request.Score ?? 0
                };
                await dbContext.UserDetails.AddAsync(userDetail);
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("添加失败");
                }
                return "添加成功";
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> DelUser(List<DelUserReq> requests)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var users = requests.MapToList<User>();
                dbContext.Users.RemoveRange(users);
                if (await dbContext.SaveChangesAsync() > 0)
                {
                    return "删除成功";
                }
                else
                {
                    throw new Exception("删除失败");
                }
            }
        }

        /// <summary>
        /// 修改用户详情信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> UpdateUserDetail(UpdateUserReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询用户
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.Id);
                var userDetail = await dbContext.UserDetails.FirstOrDefaultAsync(u => u.UserId == request.Id);
                bool isNull = userDetail == null ? true:false; //用户详情是否为空
                if (user != null)
                {
                    user.Username = request.Username ?? user.Username;
                    user.Phone = request.Phone ?? user.Phone;
                    user.Email = request.Email ?? user.Email;
                    user.Status = request.Status ?? user.Status;
                    user.Phone = request.Phone ?? user.Phone;
                    if(isNull)
                    {
                        userDetail = new UserDetail();
                        userDetail.UserId = user.Id;
                    }
                    userDetail.Nick = request.Nick ?? userDetail.Nick;
                    userDetail.Sex = request.Sex ?? userDetail.Sex;
                    userDetail.Birthday = request.Birthday == null ?  (userDetail.Birthday == null ? null : Convert.ToDateTime(userDetail.Birthday)) : Convert.ToDateTime(request.Birthday);
                    userDetail.RegisterTime = request.RegisterTime == null ? (userDetail.RegisterTime == null ? null : Convert.ToDateTime(userDetail.RegisterTime)) : Convert.ToDateTime(request.RegisterTime);
                    userDetail.Remark = request.Remark ?? userDetail.Remark;
                    userDetail.Score = request.Score ?? userDetail.Score;

                    dbContext.Entry(user).State = EntityState.Modified;
                    if (isNull) //如果为空则添加,不为空则修改
                    {
                        dbContext.Entry(userDetail).State = EntityState.Added;
                    }
                    else
                    {
                        dbContext.Entry(userDetail).State = EntityState.Modified;
                    }
                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        return "修改成功";
                    }
                    else
                    {
                        throw new Exception("修改失败");
                    }
                }
                else
                {
                    throw new Exception("用户信息异常");
                }
            }
        }
    }
}
