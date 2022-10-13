using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
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
        private readonly IDatabase cache;
        /// <summary>
        /// 序列化配置
        /// </summary>
        private readonly JsonSerializerSettings jsonConfig;
        public FollowApp(IDbContextFactory<UserDBContext> contextFactory, IConnectionMultiplexer connection)
        {
            this.contextFactory = contextFactory;
            this.cache = connection.GetDatabase(2);
            this.jsonConfig = new JsonSerializerSettings();
            this.jsonConfig.ContractResolver = new CamelCasePropertyNamesContractResolver(); //启用小驼峰格式
            this.jsonConfig.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }


        /// <summary>
        /// 关注列表
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public async Task<List<FollowView>> FollowList(List<SearchCondition> condidtion, int id,bool fans)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var follows = dbContext.UserFollows.Where(f => fans ? f.WatchId == id : f.UserId == id).Select(f => new
                {
                    Id = f.Id,
                    UserId = fans ? f.UserId : f.WatchId,
                    Username = fans ? f.User.Username : f.Watch.Username,
                    Nick = fans ? f.User.UserDetail.Nick : f.Watch.UserDetail.Nick,
                    Remark = fans ? f.User.UserDetail.Remark : f.Watch.UserDetail.Remark,
                    Photo = fans ? f.User.Photo : f.Watch.Photo
                });
                //筛选条件
                if (condidtion.Count > 0)
                {
                    foreach (var con in condidtion)
                    {
                        follows = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? follows.Where(f => f.Username.Contains(con.Value, StringComparison.OrdinalIgnoreCase)) : follows;
                        follows = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? follows.Where(f => f.Nick.Contains(con.Value, StringComparison.OrdinalIgnoreCase)) : follows;
                        follows = "Remark".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? follows.Where(f => f.Remark.Contains(con.Value, StringComparison.OrdinalIgnoreCase)) : follows;
                    }
                }
                var followView = (await follows.ToListAsync()).MapToList<FollowView>();
                return followView;
            }
        }


        /// <summary>
        /// 获取关注通知信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<FollowView>> GetFollowMessage(int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                List<FollowView> followList = new List<FollowView>();
                var followKeys = await cache.ScriptEvaluateAsync(LuaScript.Prepare($"local res = redis.call('KEYS','followNotify:{uid}*') return res"));
                if (!followKeys.IsNull)
                {
                    var userList = await dbContext.Users.Include(u => u.UserDetail).ToListAsync();
                    //获取所有的值
                    RedisKey[] keys = (RedisKey[])followKeys;
                    var values = await cache.StringGetAsync(keys);
                    foreach (var item in values)
                    {
                        var follow = JsonConvert.DeserializeObject<UserFollow>(item);
                        var user = userList.FirstOrDefault(u => u.Id == follow.UserId);
                        if (user != null)
                        {
                            followList.Add(new FollowView
                            {
                                Id = follow.Id,
                                UserId = user.Id,
                                Username = user.Username,
                                Nick = user.UserDetail.Nick,
                                Remark = user.UserDetail.Remark,
                                Photo = user.Photo
                            });
                        }
                    }
                }
                return followList;
            }
        }

        /// <summary>
        /// 删除关注信息
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="uid"></param>
        /// <param name="isAll"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> DeleteFollowMessage(int uid,int fid,bool isAll)
        {
            //已读全部
            if (isAll)
            {
                var followKeys = await cache.ScriptEvaluateAsync(LuaScript.Prepare($"local res = redis.call('KEYS','followNotify:{uid}*') return res"));
                if (!followKeys.IsNull)
                {
                    RedisKey[] k = (RedisKey[])followKeys;
                    await this.cache.KeyDeleteAsync(k);
                }
            }
            else
            {
                await cache.KeyDeleteAsync($"followNotify:{uid}:{fid}");
            }

            return "操作成功";
        }

        /// <summary>
        /// 关注/取消某人
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="sbId">被关注用户ID</param>
        /// <returns></returns>
        public async Task<string> FollowSb(int id, int sbId)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                if (id == sbId)
                {
                    throw new Exception("不能关注自己");
                }
                var follow = await dbContext.UserFollows.FirstOrDefaultAsync(f => f.UserId == id && f.WatchId == sbId);
                var message = "";
                if (follow == null)
                {
                    follow = new UserFollow
                    {
                        UserId = id,
                        WatchId = sbId,
                    };
                    dbContext.UserFollows.Add(follow);
                    message = "关注成功";
                }
                else
                {
                    dbContext.UserFollows.Remove(follow);
                    message = "取消关注成功";
                }

                if ((await dbContext.SaveChangesAsync()) > 0)
                {
                    if(message == "关注成功")
                    {
                        //推送消息
                        await cache.StringSetAsync($"followNotify:{sbId}:{follow.Id}", JsonConvert.SerializeObject(follow, jsonConfig));
                    }
                }
                else
                {
                    throw new Exception("操作失败");
                }

                return message;
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
