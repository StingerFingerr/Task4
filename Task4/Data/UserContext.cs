using Microsoft.EntityFrameworkCore;
using Task4.Models;

namespace Task4.Data
{
    public class UserContext: DbContext
    {
        public DbSet<UserModel> Users { get; set; }

        public UserContext(DbContextOptions<UserContext> options): base(options)
        {
            Database.EnsureCreated();
        }
    }
}
