using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using UserService.App.Interface;
using UserService.Domain;
using UserService.Request.Admin;
using UserService.Request.Request.Admin;
using UserService.Response;

namespace UserService.App
{
    public class AdminApp:IAdminApp
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly IDbContextFactory<UserDBContext> contextFactory;
        /// <summary>
        /// Redis缓存客户端
        /// </summary>
        private readonly IDistributedCache cache;

        public AdminApp(IDbContextFactory<UserDBContext> contextFactory, IDistributedCache cache)
        {
            this.contextFactory = contextFactory;
            this.cache = cache;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserDetailView>> GetUserDetails(SearchCondition[] condition)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var users = context.UserDetails.Select(u => new
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
                });
                //判断是否有条件
                if (condition.Length > 0)
                {
                    foreach (var con in condition)
                    {
                        users = "UserId".Equals(con.Key,StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.UserId == Convert.ToInt32(con.Value)) : users;
                        users = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Username.Contains(con.Value)) : users;
                        users = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Nick.Contains(con.Value)) : users;
                        users = "Phone".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Phone.Contains(con.Value)) : users;
                        users = "Email".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Email.Contains(con.Value)) : users;
                        users = "Sex".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Sex == Convert.ToInt32(con.Value)) : users;
                        users = "Remark".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Remark.Contains(con.Value)) : users;
                        users = "Status".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? users.Where(u => u.Status == Convert.ToInt32(con.Value)): users;
                    }
                }
                var result = await users.ToListAsync();
                var usersView = result.MapToList<UserDetailView>();
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
                var user = await context.UserDetails.Where(u => u.UserId == id).Select(u => new
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
        /// 添加用户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> AddUser(AddUserReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var user = new User()
                {
                    Username = request.Username,
                    Password = request.Password.ShaEncrypt(),
                    Phone = request.Phone,
                    Email = request.Email,
                    Status = request.Status ?? 0
                };
                dbContext.Users.Add(user);
                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("添加失败");
                }

                var userDetail = new UserDetail()
                {
                    UserId = (await dbContext.Users.FirstOrDefaultAsync(u => u.Username == user.Username)).Id,
                    Nick = request.Nick,
                    Sex = request.Sex,
                    Birthday = Convert.ToDateTime(request.Birthday),
                    RegisterTime = DateTime.Now,
                    Remark = request.Remark,
                    Score = request.Score ?? 0
                };
                dbContext.UserDetails.Add(userDetail);
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
        public async Task<string> ChangeUserDetail(ModifyUserReq request)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询用户
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.Id);
                var userDetail = await dbContext.UserDetails.FirstOrDefaultAsync(u => u.UserId == request.Id);
                if (user != null && userDetail != null)
                {
                    user.Username = request.Username ?? user.Username;
                    user.Phone = request.Phone ?? user.Phone;
                    user.Email = request.Email ?? user.Email;
                    user.Status = request.Status ?? user.Status;
                    user.Phone = request.Phone ?? user.Phone;
                    userDetail.Nick = request.Nick ?? userDetail.Nick;
                    userDetail.Sex = request.Sex ?? userDetail.Sex;
                    userDetail.Birthday = Convert.ToDateTime(request.Birthday ?? userDetail.Birthday.ToString());
                    userDetail.RegisterTime = Convert.ToDateTime(request.RegisterTime ?? userDetail.RegisterTime.ToString());
                    userDetail.Remark = request.Remark ?? userDetail.Remark;
                    userDetail.Score = request.Score ?? userDetail.Score;

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
