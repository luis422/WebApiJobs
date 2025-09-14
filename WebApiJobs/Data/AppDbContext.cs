using Microsoft.EntityFrameworkCore;
using WebApiJobs.Data.Entities;

namespace WebApiJobs.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<EmailEntity> Emails { get; set; }


        public AppDbContext() : base()
        {
        }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
