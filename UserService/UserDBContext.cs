using Microsoft.EntityFrameworkCore;
using UserService.Domain;

namespace UserService
{
    public class UserDBContext:DbContext
    {
        public UserDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
