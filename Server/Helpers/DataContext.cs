using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using csharpwebsite.Server.Entities;


namespace csharpwebsite.Server.Helpers
{
    public abstract class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<Session> SessionSlots { get; set; }
    }
}