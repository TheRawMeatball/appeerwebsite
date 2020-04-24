using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using csharpwebsite.Server.Entities;

namespace csharpwebsite.Server.Helpers
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<Session> SessionSlots { get; set; }
    }
}