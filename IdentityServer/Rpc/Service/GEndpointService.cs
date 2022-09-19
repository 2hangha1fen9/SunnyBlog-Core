using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentityService.Rpc.Protos;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using IdentityService.Domain;

namespace IdentityService.Rpc.Service
{
    /// <summary>
    /// 注册服务api端点
    /// </summary>
    public class GEndpointService:gEndpoint.gEndpointBase
    {
        //数据库上下文
        private readonly IDbContextFactory<AuthDBContext> contextFactory;
        private readonly static object obj = new object(); //线程锁

        public GEndpointService(IDbContextFactory<AuthDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public override Task<Empty> RegisterEndpoint(Endpoints request, ServerCallContext context)
        {
            lock (obj) //只有一个线程能进行注册,避免数据库死锁
            {
                using (AuthDBContext dBContext = contextFactory.CreateDbContext())
                {
                    //获取待注册的
                    var preRegister = request.Endpoint.MapToList<Permission>();
                    //已存在的服务
                    var permissions = dBContext.Permissions.ToList();
                    foreach (var pre in preRegister)
                    {
                        var p = permissions.FirstOrDefault(p => p.Service == pre.Service && p.Service == pre.Service && p.Controller == pre.Controller && p.Action == pre.Action);
                        if (p == null)
                        {
                            //端点不存在，添加
                            dBContext.Entry(pre).State = EntityState.Added;
                        }
                        else
                        {
                            //端点存在，修改
                            p.Service = pre.Service;
                            p.Controller = pre.Controller;
                            p.Action = pre.Action;
                            p.Description = pre.Description;
                            p.UpdateTime = DateTime.Now;
                            p.IsPublic = pre.IsPublic;
                            dBContext.Entry(p).State = EntityState.Modified;
                        }
                    }
                    dBContext.SaveChanges();
                }
                return Task.FromResult(new Empty());
            }
        }

        public async override Task<Endpoints> GetPublicEndpoint(Empty request, ServerCallContext context)
        {
            using (AuthDBContext dBContext = contextFactory.CreateDbContext())
            {
                //查询所有公共权限
                var publicPermission = await dBContext.Permissions.Where(p => p.IsPublic == 1 && p.Status == 1).ToListAsync();
                //构造返回体
                Endpoints endpoints = new Endpoints();
                foreach (var permission in publicPermission)
                {
                    //添加对象到返回体
                    Protos.Endpoint endpoint = permission.MapTo<Protos.Endpoint>();
                    endpoints.Endpoint.Add(endpoint);
                }
                return endpoints;
            }
        }
    }
}
