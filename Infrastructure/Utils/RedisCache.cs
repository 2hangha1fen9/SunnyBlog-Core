using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    /// <summary>
    /// Redis缓存标识
    /// </summary>
    public class RedisCache : Attribute, IAsyncResourceFilter
    {
        /// <summary>
        /// 缓存过期时间
        /// </summary>
        public int Expiration { get; set; }
        /// <summary>
        /// 自定义缓存键
        /// </summary>
        public string CacheKey { get; set; }
        /// <summary>
        /// redis库号
        /// </summary>
        public int DatabaseNum { get; set; }
        /// <summary>
        /// 序列化配置
        /// </summary>
        private readonly JsonSerializerSettings jsonConfig;
        /// <summary>
        /// Redis客户端,依赖注入
        /// </summary>
        private readonly IDatabase database;

        public RedisCache(IConnectionMultiplexer connection,string? CacheKey = "", int? Expiration = 360,int? databaseNum = 1)
        {
            this.Expiration = Expiration.Value;
            this.CacheKey = CacheKey;
            this.DatabaseNum = databaseNum.Value;
            this.jsonConfig = new JsonSerializerSettings();
            this.jsonConfig.ContractResolver = new CamelCasePropertyNamesContractResolver(); //启用小驼峰格式
            this.database = connection.GetDatabase(DatabaseNum);
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            //获取缓存键,默认按api路径缓存
            if (string.IsNullOrWhiteSpace(CacheKey))
            {
                CacheKey =$"{context.HttpContext.Request.Path.Value}{context.HttpContext.Request.QueryString}";
            }

            //从Redis中获取值
            var cache = await database.StringGetAsync(CacheKey);
            if (cache != RedisValue.Null)
            {
                //命中缓存，返回缓存中的内容
                context.Result = new ContentResult()
                {
                    Content = cache
                };
            }
            else
            {
                //未命中缓存，将查询结果存入redis
                var result = await next.Invoke();
                var content = ((ObjectResult)result.Result).Value;
                //序列化
                var json = JsonConvert.SerializeObject(content, jsonConfig);
                await database.StringSetAsync(context.HttpContext.Request.Path.Value, json, TimeSpan.FromSeconds(Expiration));
            }
        }
    }

    /// <summary>
    /// 刷新缓存标识
    /// </summary>
    public class RedisFlush : Attribute, IResultFilter
    {
        /// <summary>
        /// 自定义缓存键
        /// </summary>
        public string CacheKey { get; set; }
        /// <summary>
        /// redis库号
        /// </summary>
        public int DatabaseNum { get; set; }
        /// <summary>
        /// Redis客户端,依赖注入
        /// </summary>
        private readonly IDatabase database;
        public RedisFlush(IConnectionMultiplexer connection, string cacheKey, int? databaseNum = 1)
        {
            this.CacheKey = cacheKey;
            this.DatabaseNum = databaseNum.Value;
            this.database = connection.GetDatabase(DatabaseNum);
        }

        /// <summary>
        /// 当完成修改操作后,剔除缓存
        /// </summary>
        /// <param name="context"></param>
        public async void OnResultExecuting(ResultExecutingContext context)
        {
            await database.KeyDeleteAsync(CacheKey);
        }

        public async void OnResultExecuted(ResultExecutedContext context)
        {

        }
    }
}
