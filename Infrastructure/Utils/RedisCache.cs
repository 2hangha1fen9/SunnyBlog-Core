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
    /// Redis缓存
    /// </summary>
    public class RedisCache: Attribute, IAsyncResourceFilter
    {
        /// <summary>
        /// 缓存过期时间
        /// </summary>
        private int Expiration { get; set; } = 100;
        /// <summary>
        /// 序列化配置
        /// </summary>
        private static JsonSerializerSettings jsonConfig;
        

        /// <summary>
        /// Redis客户端
        /// </summary>
        private static  ConnectionMultiplexer connection;
        private static IDatabase database;

        public RedisCache(){}
        public RedisCache(string address)
        {
            connection = ConnectionMultiplexer.Connect(address);
            database = connection.GetDatabase(1);
            SetJsonConfig();
        }

        public RedisCache(int expiration)
        {
            this.Expiration = expiration;
            SetJsonConfig();
        }

        /// <summary>
        /// 配置Json格式
        /// </summary>
        private void SetJsonConfig()
        {
            jsonConfig = new JsonSerializerSettings();
            jsonConfig.ContractResolver = new CamelCasePropertyNamesContractResolver(); //启用小驼峰格式
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
                //序列化
                var json = JsonConvert.SerializeObject(content, jsonConfig); 
                await database.StringSetAsync(context.HttpContext.Request.Path.Value,json,TimeSpan.FromSeconds(Expiration));
            }
        }
    }
}
