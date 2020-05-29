using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace csharpwebsite.Server.Helpers
{
    public class MariaDBDataContext : DataContext
    {
        public MariaDBDataContext(IConfiguration configuration) : base(configuration) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            //"Server=172.17.0.2;Database=Main;uid=root;Password=mypwd;"
            options.UseMySql(Configuration.GetConnectionString("WebApiDatabase"), mySqlOptions => mySqlOptions
                    .ServerVersion(new Version(10 , 4), ServerType.MariaDb)
                    .CharSet(CharSet.Latin1)
                    .EnableRetryOnFailure(5)
            );
        }
    }
}