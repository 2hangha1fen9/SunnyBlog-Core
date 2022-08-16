using Consul;
using Microsoft.AspNetCore.Builder;


namespace ConsulBuilder
{
    //注册中心自动注册服务,中间件
    public static class ConsulRegistrationExtensions
    {
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, ConsulServiceOptions consulServiceOptions)
        {
            //注册中心客户端
            var consulClient = new ConsulClient(c =>
            {
                //配置注册中心地址
                c.Address = new Uri(consulServiceOptions.ConsulAddress);
            });
            //注册服务
            var registration = new AgentServiceRegistration()
            {
                ID = Guid.NewGuid().ToString(),
                Name = consulServiceOptions.ServiceName, //服务名
                Address = consulServiceOptions.ServiceIP, //注册中心地址
                Port = consulServiceOptions.ServicePort, //注册中心端口号
                Check = new AgentServiceCheck()  //健康检查配置
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5), //服务启动后多久注册   
                    Interval = TimeSpan.FromSeconds(10), //健康检查间隔
                    HTTP = consulServiceOptions.ServiceHealthCheck, //健康检查地址
                    Timeout = TimeSpan.FromSeconds(5) //超时时间
                }
            };
            consulClient.Agent.ServiceRegister(registration).Wait(); //注册服务
            return app;
        }
    }
}