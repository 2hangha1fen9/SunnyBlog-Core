using ArticleService.App.Interface;
using ArticleService.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.App
{
    public class SettingApp:ISettingApp
    {
        private readonly IDbContextFactory<ArticleDBContext> contextFactory;

        public SettingApp(IDbContextFactory<ArticleDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        //列出设置项
        public async Task<List<GlobalSetting>> ListSetting()
        {
            using(var dbContext = contextFactory.CreateDbContext())
            {
                return await dbContext.GlobalSettings.ToListAsync();  
            }
        }

        //设置值
        public async Task<string> SetValue(string key, int value)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var setting = await dbContext.GlobalSettings.FirstOrDefaultAsync(s => s.Option == key);
                if (setting != null)
                {
                    setting.Value = value;
                    await dbContext.SaveChangesAsync();
                }
                return "设置成功";
            }
        }
    }
}
