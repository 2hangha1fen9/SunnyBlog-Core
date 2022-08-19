using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Namotion.Reflection;
using Microsoft.AspNetCore.Builder;
using Infrastructure.Auth.Protos;
using Microsoft.Extensions.Configuration;
using Grpc.Net.Client;
using Google.Protobuf.Collections;

namespace Infrastructure
{
    public static class PermissionRegistrar
    {
        /// <summary>
        /// 注册当前服务的所有api
        /// </summary>
        /// <typeparam name="T">当前配置类Program</typeparam>
        /// <param name="app"></param>
        /// <param name="config">应用配置</param>
        /// <returns></returns>
        public async static Task<IApplicationBuilder> UsePermissionRegistrar<T>(this IApplicationBuilder app, string consulAddress)
        {
            //延迟5秒注册
            Thread.Sleep(5000);
            //获取认证服务地址
            var url = ServiceUrl.GetServiceUrlByName("IdentityService", consulAddress);
            //创建频道
            using var channel = GrpcChannel.ForAddress(url);
            //创建客户端
            var client = new gEndpoint.gEndpointClient(channel);
            //获取当前程序集
            var controllers = typeof(T). //获取这个类型的Type实例
                Assembly. //获取当前所有程序集
                GetTypes(). //获取所有Type实例
                AsEnumerable(). //转换为可迭代对象
                Where(t => typeof(ControllerBase).IsAssignableFrom(t)). //判断对应类型是否是控制器
                ToList(); //转换为集合
            //创建消息对象
            Endpoints endpoints = new Endpoints();
            foreach (Type type in controllers)
            {
                if (type.Name == "HealthCheckController") //跳过健康检查控制器
                {
                    continue;
                }
                //遍历控制的所有方法
                var methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
                foreach (MethodInfo method in methods) 
                {
                    Endpoint endpoint = new Endpoint();
                    endpoint.Service = type.Assembly.GetName().Name.ToLower(); //获取当前程序集名称
                    endpoint.Controller = type.Name.Replace("Controller","").ToLower(); //获取当前控制器名称
                    endpoint.Action = method.Name.ToLower(); //获取当前操作方法
                    endpoint.Description = method.GetXmlDocsSummary(); //获取控制器上的注释
                    endpoints.Endpoint.Add(endpoint); //添加消息对象
                }
            }
 
            //调用gRPC
            await client.RegisterEndpointAsync(endpoints);
            return app;
        }
    }
}
