using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using csharpwebsite.Server.Entities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

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
            options.UseMySql(Configuration.GetConnectionString("WebApiDatabase"), mySqlOptions => mySqlOptions
                    // replace with your Server Version and Type
                    .ServerVersion(new Version(10 , 4), ServerType.MariaDb)
            );
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<Session> SessionSlots { get; set; }
    }
}