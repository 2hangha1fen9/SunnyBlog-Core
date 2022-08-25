using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Infrastructure.Auth.Protos;
using Infrastructure.Consul;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Auth
{
    /// <summary>
    /// RBAC授权控制处理
    /// </summary>
    public class RBACRequirementHandler : AuthorizationHandler<RBACRequirement>
    {
        /// <summary>
        /// Http上下文
        /// </summary>
        private readonly IHttpContextAccessor httpContextAccessor;
        /// <summary>
        /// 获取配置文件
        /// </summary>
        private readonly IConfiguration config;
        /// <summary>
        /// Redis连接
        /// </summary>
        private readonly IDatabase database1;

        public RBACRequirementHandler(IHttpContextAccessor httpContextAccessor, IConnectionMultiplexer cache, IConfiguration config)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.config = config;
            this.database1 = cache.GetDatabase(0);
        }

        /// <summary>
        /// 授权处理逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, RBACRequirement requirement)
        {
            //获取router对象
            var router = httpContextAccessor.HttpContext?.GetRouteData();
            //获取Controller、Action
            var currentController = router?.Values["controller"]?.ToString();
            var currentAction = router?.Values["action"]?.ToString();
            if (!string.IsNullOrWhiteSpace(currentController) && !string.IsNullOrWhiteSpace(currentAction))
            {
                //获取token权限信息
                var permissions = context.User?.Claims?.FirstOrDefault(c => c.Type == "permission")?.Value;
                var userId = context.User?.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                List<Permission> permissionsList;
                try
                {
                    permissionsList = JsonConvert.DeserializeObject<List<Permission>>(permissions);
                }
                catch (Exception)//尝试查询公共权限
                {
                    //尝试调用Redis获取公共权限
                    var value = await database1.StringGetAsync("publicPermission");
                    if (!string.IsNullOrEmpty(value))
                    {
                        permissionsList = JsonConvert.DeserializeObject<List<Permission>>(value);
                    }
                    else //未命中缓存调用RPC查询
                    {
                        //调用consul服务发现，获取rpc服务地址
                        var url = ServiceUrl.GetServiceUrlByName("IdentityService",
                                    config.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress);
                        //创建通讯频道
                        using var channel = GrpcChannel.ForAddress(url);
                        //创建客户端
                        var client = new gEndpoint.gEndpointClient(channel);
                        //远程调用
                        var publicPermission = await client.GetPublicEndpointAsync(new Empty());
                        //映射结果
                        permissionsList = publicPermission.Endpoint.MapToList<Permission>();
                        //将结果存入缓存
                        await database1.StringSetAsync("publicPermission", JsonConvert.SerializeObject(permissionsList));
                    }
                }
                //查询当前请求的权限
                var requestPermission = permissionsList.Find(p => (("*".Equals(p.Controller, StringComparison.InvariantCultureIgnoreCase) || currentController.Equals(p.Controller, StringComparison.OrdinalIgnoreCase)) &&
                                                ("*".Equals(p.Action, StringComparison.OrdinalIgnoreCase) || currentAction.Equals(p.Action, StringComparison.OrdinalIgnoreCase))));
                if (requestPermission != null) //查询到权限放行
                {
                    context.Succeed(requirement); //放行请求
                }
                else
                {
                    context.Fail(); //拒绝请求
                }
            }
            else
            {
                context.Fail(); //拒绝请求
            }
        }
    }
}
