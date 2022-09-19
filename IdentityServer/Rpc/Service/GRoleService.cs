using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentityService.Domain;
using IdentityService.Rpc.Protos;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Rpc.Service
{
    public class GRoleService:gRole.gRoleBase
    {
        //数据库上下文
        private readonly IDbContextFactory<AuthDBContext> contextFactory;

        public GRoleService(IDbContextFactory<AuthDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        //绑定用户默认角色
        public async override Task<Empty> BindDefaultRole(BindDefaultRequest request, ServerCallContext context)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询所有默认角色
                var defaultRoles = await dbContext.Roles.Where(r => r.IsDefault == 1).ToListAsync();
                //绑定默认角色
                foreach (var role in defaultRoles)
                {
                    var r = new UserRoleRelation
                    {
                        UserId = request.Uid,
                        RoleId = role.Id,
                    };
                    dbContext.UserRoleRelations.Add(r);
                }
                await dbContext.SaveChangesAsync();
                return new Empty();
            }
        }
    }
}
