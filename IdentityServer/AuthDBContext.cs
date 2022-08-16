using Microsoft.EntityFrameworkCore;
using Service.IdentityService.Domain;

namespace Service.IdentityService
{
    public class AuthDBContext:DbContext
    {
        public AuthDBContext(DbContextOptions options) : base(options)
        {
          
        }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRoleRelation> UserRoleRelations { get; set; }
        public DbSet<RolePermissionRelation> RolePermissionRelations { get; set; }
    }
}
