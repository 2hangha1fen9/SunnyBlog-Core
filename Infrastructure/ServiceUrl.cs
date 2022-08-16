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
            if (services != null)
            {
                //轮询获取地址
                var index = number++ % services.Count;
                var result = services[index];
                return $"https://{result.Address}:{result.Port}";
            }
            else
            {
                throw new Exception("没有找到地址");
            }
        }
    }
}
