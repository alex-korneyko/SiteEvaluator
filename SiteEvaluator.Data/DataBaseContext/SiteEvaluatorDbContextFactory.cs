using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SiteEvaluator.Data.DataBaseContext
{
    public class SiteEvaluatorDbContextFactory : IDesignTimeDbContextFactory<SiteEvaluatorDbContext>
    {
        public SiteEvaluatorDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SiteEvaluatorDbContext>();
            
            if (args.Length < 1 || string.IsNullOrEmpty(args[0]))
            {
                throw new ArgumentException(
                    "Invalid connection string value. Use CLI command -> dotnet ef database update -- \"db_connection_string\"");
            }
            
            optionsBuilder.UseSqlServer(args[0]);
            
            return new SiteEvaluatorDbContext(optionsBuilder.Options);
        }
    }
}