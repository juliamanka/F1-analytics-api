using Microsoft.EntityFrameworkCore;
using F1Analytics.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace F1Analytics.Database
{
    public class F1AnalyticsDbContext : IdentityDbContext<User>
    {
        public F1AnalyticsDbContext(DbContextOptions<F1AnalyticsDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Series> Series { get; set; }
        public DbSet<Measurement> Measurements { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // -------------------------------
    // Series Entity Configuration
    // -------------------------------
    modelBuilder.Entity<Series>(entity =>
    {
        entity.ToTable("Series");

        entity.HasIndex(e => e.SeriesId)
            .IsUnique()
            .HasDatabaseName("IX_Series_SeriesId");

        entity.Property(e => e.SeriesId)
            .HasColumnName("series_id");

        entity.Property(e => e.MinValue)
            .HasColumnName("min_value")
            .HasColumnType("decimal(10,2)");

        entity.Property(e => e.MaxValue)
            .HasColumnName("max_value")
            .HasColumnType("decimal(10,2)");

        entity.Property(e => e.MeasurementType)
            .HasColumnName("measurement_type");

        entity.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        entity.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp")
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
    });

    // -------------------------------
    // Measurement Entity Configuration
    // -------------------------------
    modelBuilder.Entity<Measurement>(entity =>
    {
        entity.ToTable("Measurements");

        entity.HasIndex(e => new { e.SeriesId, e.Timestamp })
            .HasDatabaseName("idx_series_timestamp");

        entity.HasIndex(e => e.Timestamp)
            .HasDatabaseName("idx_timestamp");

        entity.HasIndex(e => e.SeriesId)
            .HasDatabaseName("idx_series_id");

        entity.Property(e => e.SeriesId)
            .HasColumnName("series_id");

        entity.Property(e => e.Timestamp)
            .HasColumnName("timestamp");

        entity.Property(e => e.Value)
            .HasColumnName("value")
            .HasColumnType("decimal(10,2)");

        entity.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        entity.HasOne(d => d.Series)
            .WithMany(p => p.Measurements)
            .HasPrincipalKey(p => p.SeriesId)
            .HasForeignKey(d => d.SeriesId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Measurements_Series");
    });

    // -------------------------------
    // User Entity Configuration
    // -------------------------------
    modelBuilder.Entity<User>(entity =>
    {
        entity.Property(e => e.CreatedAt)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        entity.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp")
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
    });

    // -------------------------------
    // Seed Data
    // -------------------------------
    SeedInitialData(modelBuilder);
}

private void SeedInitialData(ModelBuilder modelBuilder)
{
    // fixed reference date for deterministic seeding
    var seedDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);

    // --- Roles ---
    modelBuilder.Entity<IdentityRole>().HasData(
        new IdentityRole
        {
            Id = "admin-role-id-guid",
            Name = "Admin",
            NormalizedName = "ADMIN"
        },
        new IdentityRole
        {
            Id = "reader-role-id-guid",
            Name = "Reader",
            NormalizedName = "READER"
        }
    );

    // --- Users ---
    modelBuilder.Entity<User>().HasData(
        new User
        {
            Id = "admin-user-id-guid",
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@f1aerodynamics.com",
            NormalizedEmail = "ADMIN@F1AERODYNAMICS.COM",
            EmailConfirmed = true,
            PasswordHash = "AQAAAAIAAYagAAAAEOQtfxf1oyqfF4xpCLuAJEY/pL3WfzF+N8sEdO4bz7fhJPxnFC6JGO3gi4GUGTmjuw==",
            SecurityStamp = "static-admin-security-stamp",
            CreatedAt = seedDate,
            UpdatedAt = seedDate
        },
        new User
        {
            Id = "reader-user-id-guid",
            UserName = "reader",
            NormalizedUserName = "READER",
            Email = "reader@f1aerodynamics.com",
            NormalizedEmail = "READER@F1AERODYNAMICS.COM",
            EmailConfirmed = true,
            PasswordHash = "AQAAAAIAAYagAAAAEOQtfxf1oyqfF4xpCLuAJEY/pL3WfzF+N8sEdO4bz7fhJPxnFC6JGO3gi4GUGTmjuw==",
            SecurityStamp = "static-reader-security-stamp",
            CreatedAt = seedDate,
            UpdatedAt = seedDate
        }
    );

    // --- UserRoles ---
    modelBuilder.Entity<IdentityUserRole<string>>().HasData(
        new IdentityUserRole<string>
        {
            RoleId = "admin-role-id-guid",
            UserId = "admin-user-id-guid"
        },
        new IdentityUserRole<string>
        {
            RoleId = "reader-role-id-guid",
            UserId = "reader-user-id-guid"
        }
    );

    // --- Series Data ---
    modelBuilder.Entity<Series>().HasData(
        new Series
        {
            Id = 1,
            SeriesId = "baseline_config",
            Name = "Baseline Front Wing Configuration",
            Description = "Standard wing setup - reference configuration",
            MinValue = -550m,
            MaxValue = -350m,
            Unit = "N",
            Color = "#FF6B6B",
            MeasurementType = "Front Wing Downforce",
            CreatedAt = seedDate,
            UpdatedAt = seedDate
        },
        new Series
        {
            Id = 2,
            SeriesId = "high_downforce_config",
            Name = "High Downforce Front Wing",
            Description = "Modified wing with increased angle of attack",
            MinValue = -650m,
            MaxValue = -400m,
            Unit = "N",
            Color = "#4ECDC4",
            MeasurementType = "Front Wing Downforce",
            CreatedAt = seedDate,
            UpdatedAt = seedDate
        },
        new Series
        {
            Id = 3,
            SeriesId = "low_drag_config",
            Name = "Low Drag Front Wing",
            Description = "Reduced downforce for high-speed circuits",
            MinValue = -480m,
            MaxValue = -280m,
            Unit = "N",
            Color = "#45B7D1",
            MeasurementType = "Front Wing Downforce",
            CreatedAt = seedDate,
            UpdatedAt = seedDate
        },
        new Series
        {
            Id = 4,
            SeriesId = "wet_weather_config",
            Name = "Wet Weather Front Wing",
            Description = "Special configuration for rain conditions",
            MinValue = -600m,
            MaxValue = -380m,
            Unit = "N",
            Color = "#96CEB4",
            MeasurementType = "Front Wing Downforce",
            CreatedAt = seedDate,
            UpdatedAt = seedDate
        },
        new Series
        {
            Id = 5,
            SeriesId = "experimental_config",
            Name = "Experimental Front Wing Design",
            Description = "New aerodynamic concept testing",
            MinValue = -580m,
            MaxValue = -350m,
            Unit = "N",
            Color = "#FFEAA7",
            MeasurementType = "Front Wing Downforce",
            CreatedAt = seedDate,
            UpdatedAt = seedDate
        }
    );
}

/// <summary>
        /// Override SaveChanges to automatically update timestamps
        /// </summary>
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        /// <summary>
        /// Override SaveChangesAsync to automatically update timestamps
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates timestamps for entities that have UpdatedAt property
        /// </summary>
        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Entity is Series series)
                {
                    if (entry.State == EntityState.Added)
                        series.CreatedAt = DateTime.UtcNow;
                    series.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is User user)
                {
                    if (entry.State == EntityState.Added)
                        user.CreatedAt = DateTime.UtcNow;
                    user.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is Measurement measurement && entry.State == EntityState.Added)
                {
                    measurement.CreatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
