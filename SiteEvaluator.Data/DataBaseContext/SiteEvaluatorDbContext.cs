using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SiteEvaluator.Data.Model;

namespace SiteEvaluator.Data.DataBaseContext
{
    public class SiteEvaluatorDbContext :
        DbContext,
        IEfRepositoryDbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<TargetHost> TargetHosts { get; set; }
        public DbSet<PageInfo> PageInfos { get; set; }
        public DbSet<PageInfoUrl> PageInfoUrls { get; set; }

        public SiteEvaluatorDbContext(DbContextOptions options) : base(options)
        {
        }

        public SiteEvaluatorDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_configuration == null)
                return;
            
            var connectionString = _configuration.GetConnectionString("developmentDb");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderHandler.DefineNavigations(modelBuilder);
        }
    }
}