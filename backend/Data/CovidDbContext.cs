using CovidDashboard.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CovidDashboard.Api.Data
{
    public class CovidDbContext : DbContext
    {
        public CovidDbContext(DbContextOptions<CovidDbContext> options)
            : base(options) { }

        public DbSet<CovidRecord> CovidRecords { get; set; } = null!;
        public DbSet<DailyReportUs> DailyReportsUs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CovidRecord>(entity =>
            {
                entity.HasIndex(e => e.CountryRegion);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => new { e.CountryRegion, e.Date });
            });

            modelBuilder.Entity<DailyReportUs>(entity =>
            {
                entity.HasIndex(e => e.ProvinceState);
                entity.HasIndex(e => e.Date);
            });
        }
    }
}
