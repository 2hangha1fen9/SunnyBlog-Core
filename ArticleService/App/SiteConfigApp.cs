using ArticleService.App.Interface;
using StackExchange.Redis;

namespace ArticleService.App
{
    public class SiteConfigApp:ISiteConfigApp
    {
        private readonly IDatabase cache;
        public SiteConfigApp(IConnectionMultiplexer connection)
        {
            this.cache = connection.GetDatabase(2);
        }

        /// <summary>
        /// 获取网站脚注内容
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> GetConfig(string key)
        {
            var value = await cache.StringGetAsync($"siteconfig:{key}");
            return value;
        }

        /// <summary>
        /// 设置网站配置
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> SetConfig(string key,string value)
        {
            await cache.StringSetAsync($"siteconfig:{key}",value);
            return "设置成功";
        }

        /// <summary>
        /// 删除网站配置
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> DelConfig(string key)
        {
            await cache.KeyDeleteAsync($"siteconfig:{key}");
            return "删除成功";
        }
    }
}
