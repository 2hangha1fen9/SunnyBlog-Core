using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentityService.Rpc.Protos;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using IdentityService;
using IdentityService.Domain;

namespace IdentityService.Rpc.Service
{
    /// <summary>
    /// 注册服务api端点
    /// </summary>
    public class GEndpointService:gEndpoint.gEndpointBase
    {
        //锁，保证线程安全
        private static readonly object lockObj = new object();
        //数据库上下文
        private readonly IDbContextFactory<AuthDBContext> contextFactory;

        public GEndpointService(IDbContextFactory<AuthDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async override Task<Empty> RegisterEndpoint(Endpoints request, ServerCallContext context)
        {
            using (AuthDBContext dBContext = contextFactory.CreateDbContext())
            {
                //获取待注册的
                var preRegister = request.Endpoint.MapToList<Permission>();
                //已存在的服务
                var permissions = await dBContext.Permissions.ToListAsync();
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
                        dBContext.Entry(p).State = EntityState.Modified;
                    }
                }
                await dBContext.SaveChangesAsync();
            }
            return new Empty();
        }
    }
}
