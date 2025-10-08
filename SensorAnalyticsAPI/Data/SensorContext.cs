using Microsoft.EntityFrameworkCore;
using SensorAnalyticsAPI.Models;

namespace SensorAnalyticsAPI.Data
{
    public class SensorContext : DbContext
    {
        public SensorContext(DbContextOptions<SensorContext> options) : base(options)
        {
        }

        public DbSet<SensorReading> SensorReadings { get; set; }
        public DbSet<SensorStatistics> SensorStatistics { get; set; }
        public DbSet<AnomalyAlert> AnomalyAlerts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure SensorReading
            modelBuilder.Entity<SensorReading>(entity =>
            {
                entity.HasKey(e => new { e.SensorId, e.Timestamp });
                entity.Property(e => e.SensorId).HasMaxLength(50);
                entity.Property(e => e.Type).HasConversion<int>();
                entity.Property(e => e.Value).HasPrecision(18, 6);
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => e.Type);
            });

            // Configure SensorStatistics
            modelBuilder.Entity<SensorStatistics>(entity =>
            {
                entity.HasKey(e => new { e.SensorId, e.Type });
                entity.Property(e => e.SensorId).HasMaxLength(50);
                entity.Property(e => e.Type).HasConversion<int>();
                entity.Property(e => e.AverageValue).HasPrecision(18, 6);
                entity.Property(e => e.MinValue).HasPrecision(18, 6);
                entity.Property(e => e.MaxValue).HasPrecision(18, 6);
                entity.Property(e => e.StandardDeviation).HasPrecision(18, 6);
            });

            // Configure AnomalyAlert
            modelBuilder.Entity<AnomalyAlert>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.SensorId).HasMaxLength(50);
                entity.Property(e => e.Type).HasConversion<int>();
                entity.Property(e => e.Value).HasPrecision(18, 6);
                entity.Property(e => e.ExpectedValue).HasPrecision(18, 6);
                entity.Property(e => e.Deviation).HasPrecision(18, 6);
                entity.Property(e => e.Message).HasMaxLength(500);
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Severity);
            });
        }
    }
}
