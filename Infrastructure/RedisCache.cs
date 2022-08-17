using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    /// <summary>
    /// Redis缓存
    /// </summary>
    public class RedisCache: Attribute, IAsyncResourceFilter
    {
        /// <summary>
        /// 缓存过期时间
        /// </summary>
        private int Expiration { get; set; } = 100;

        private static  ConnectionMultiplexer connection;
        private readonly IDatabase database;

        public RedisCache()
        {
            connection = ConnectionMultiplexer.Connect("localhost:6379");
            database = connection.GetDatabase();
        }
        public RedisCache(int expiration):base()
        {
            this.Expiration = expiration;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            //获取请求路径
            var path = context.HttpContext.Request.Path.Value;
            //从Redis中获取值
            var cache = await database.StringGetAsync(path);
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
                await database.StringSetAsync(context.HttpContext.Request.Path.Value,JsonConvert.SerializeObject(content));
            }
        }
    }
}
