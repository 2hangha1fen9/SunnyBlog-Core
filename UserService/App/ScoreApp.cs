using Microsoft.EntityFrameworkCore;
using UserService.App.Interface;
using UserService.Domain;
using UserService.Request;

namespace UserService.App
{
    public class ScoreApp : IScoreApp
    {
        private readonly IDbContextFactory<UserDBContext> contextFactory;

        public ScoreApp(IDbContextFactory<UserDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// 增加积分
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> Increase(int id,string reason)
        {
            try
            {
                using (var dbContext = contextFactory.CreateDbContext())
                {
                    var trans = dbContext.Database.BeginTransaction();
                    var scoreUnit = await dbContext.ScoreUnits.FirstOrDefaultAsync(u => u.Name == reason);
                    //添加积分记录
                    var score = new UserScore()
                    {
                        UserId = id,
                        Value = scoreUnit.Value,
                        Reason = scoreUnit.Name,
                        Time = DateTime.Now
                    };
                    dbContext.UserScores.Add(score);
                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        //计算总积分
                        var user = await dbContext.UserDetails.FirstOrDefaultAsync(u => u.UserId == id);
                        if (user != null)
                        {
                            //获取当前积分数
                            var sum = dbContext.UserScores.Sum(u => u.Value);
                            user.Score = Convert.ToDecimal(sum);
                            //保存结果
                            if(await dbContext.SaveChangesAsync() > 0)
                            {
                                await trans.CommitAsync();
                                return "增加成功";
                            }
                        }
                    }
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new Exception("增加失败");
            }
        }

        /// <summary>
        /// 判断用户是否可以增加积分
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public async Task<bool> CanIncrease(int id, string reason)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询用户今天的积分明细
                var userScore = await dbContext.UserScores.Where(s => s.UserId == id && s.Reason == reason && s.Time.Day == DateTime.Now.Day).ToListAsync();
                if (reason == "签到") //每天只能签到一次获得积分
                {
                    return userScore.Count <= 0;
                }
                else if (reason == "评论") //每天只能评论3条获得积分
                {
                    return userScore.Count <= 2;
                }
                else if (reason == "点赞") //每天只能点赞3条获得积分
                {
                    return userScore.Count <= 2;
                }
                else if (reason == "收藏") //每天只能收藏3条获得积分
                {
                    return userScore.Count <= 2;
                }
                else if (reason == "被点赞")
                {
                    return true;
                }
                else if (reason == "被收藏")
                {
                    return true;
                }
                else if (reason == "发表文章")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    
        /// <summary>
        /// 列出所有积分单位
        /// </summary>
        /// <returns></returns>
        public async Task<List<ScoreUnit>> ListUnit()
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                return await dbContext.ScoreUnits.ToListAsync();
            }
        }

        /// <summary>
        /// 更新积分单位
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<string> SetUnit(string key, decimal value)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var units = await dbContext.ScoreUnits.FirstOrDefaultAsync(s => s.Name == key);
                if (units != null)
                {
                    units.Value = value;
                    await dbContext.SaveChangesAsync();
                }
                return "设置成功";
            }
        }
    }
}
