using Microsoft.EntityFrameworkCore;
using SiteEvaluator.Data.Model;

namespace SiteEvaluator.Data.DataBaseContext
{
    public static class ModelBuilderHandler
    {
        public static void DefineNavigations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PageInfo>()
                .HasMany(pageInfo => pageInfo.PageInfoUrls)
                .WithOne(pageInfoUrl => pageInfoUrl.PageInfo)
                .HasForeignKey(pageInfoUrl => pageInfoUrl.PageInfoId);

            modelBuilder.Entity<TargetHost>()
                .HasMany(host => host.PageInfos)
                .WithOne(pageInfo => pageInfo.TargetHost)
                .HasForeignKey(pageInfo => pageInfo.TargetHostId);
        }
    }
}