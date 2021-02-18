using Microsoft.EntityFrameworkCore;
namespace LoginRegWDb.Models
{
public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<RegUser> Users { get; set; }
    }
}