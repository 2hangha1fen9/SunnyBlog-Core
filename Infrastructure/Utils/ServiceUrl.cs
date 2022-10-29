using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ServiceUrl
    {
        //锁，保证线程安全
        private static readonly object lockObj = new object();
        public static int number = 1;
        
        
        //通过consul查询对应的服务地址，方便rpc调用
        public static string GetServiceUrlByName(string name,string consulAddress)
        {
            //创建consul客户端对象
            var consulClient = new ConsulClient(c => c.Address = new Uri(consulAddress));
            //获取consul注册的服务地址
            var agent = consulClient.Agent.Services().Result.Response.Values.ToList();
            //查询对应服务地址
            var services = agent.Where(a => a.Service == name).ToList();

            //重试次数
            int count = 10;
            while (count > 0)
            {
                if (services.Count != 0) {
                    //轮询获取地址
                    var index = number++ % services.Count;
                    var result = services[index];
                    //rpc端口号为高位端口获取时区分是否是rpc端口
                    return $"https://{result.Address}:{result.Port}";
                }
                else
                {
                    //重试获取服务地址
                    count--;
                    Thread.Sleep(2000);
                    agent = consulClient.Agent.Services().Result.Response.Values.ToList();
                    services = agent.Where(a => a.Service == name).ToList();
                }
            }
            throw new Exception("没有找到地址");
        }
    }
}
