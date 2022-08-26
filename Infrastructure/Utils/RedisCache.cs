using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

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
            try
            {
                //获取请求路径
                var path = context.HttpContext.Request.Path.Value;
                //获取查询参数
                var query = context.HttpContext.Request.QueryString;
                //获取body Hash
                context.HttpContext.Request.EnableBuffering(); //让body可以被重复读取
                using var bodyStream = new StreamReader(context.HttpContext.Request.Body);
                var body = await bodyStream.ReadToEndAsync();
                body = string.IsNullOrWhiteSpace(body) ? null : body.ShaEncrypt();
                context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);//重置流偏移量 
                
                //获取缓存键,默认按api路径缓存
                if (string.IsNullOrWhiteSpace(CacheKey))
                {
                    CacheKey = $"{path}:{query}:{body}";
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
                    await database.StringSetAsync(CacheKey, json, TimeSpan.FromSeconds(Expiration));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
        public string[] CacheKeys { get; set; }
        /// <summary>
        /// redis库号
        /// </summary>
        public int DatabaseNum { get; set; }
        /// <summary>
        /// Redis客户端,依赖注入
        /// </summary>
        private readonly IDatabase database;
        public RedisFlush(IConnectionMultiplexer connection, string[] cacheKeys, int? databaseNum = 1)
        {
            this.CacheKeys = cacheKeys;
            this.DatabaseNum = databaseNum.Value;
            this.database = connection.GetDatabase(DatabaseNum);
        }

        /// <summary>
        /// 当完成修改操作后,剔除缓存
        /// </summary>
        /// <param name="context"></param>
        public async void OnResultExecuting(ResultExecutingContext context)
        {
            //查询所有匹配的键
            foreach (var key in CacheKeys)
            {
                var result = await database.ScriptEvaluateAsync(LuaScript.Prepare($"local res = redis.call('KEYS','{key}') return res"));
                if (!result.IsNull)
                {
                    RedisKey[] k = (RedisKey[])result;
                    this.database.KeyDelete(k);
                }
            }
        }

        public async void OnResultExecuted(ResultExecutedContext context)
        {

        }
    }
}
