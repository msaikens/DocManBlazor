#nullable enable
using DossierTimeline.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DossierTimeline.Web.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Setting> Settings => Set<Setting>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // Document
        b.Entity<Document>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.FileName).IsRequired().HasMaxLength(260);
            e.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(260);
            e.Property(x => x.ContentType).HasMaxLength(127);
            e.Property(x => x.StoredPath).IsRequired().HasMaxLength(512);
            e.Property(x => x.HashSha256).HasMaxLength(64);
            e.Property(x => x.Category).HasMaxLength(128);
            e.Property(x => x.Tags).HasMaxLength(2048); // semicolon-delimited or use join table
            e.Property(x => x.Notes).HasMaxLength(4000);
            e.HasIndex(x => x.EventDateUtc);
            e.HasIndex(x => x.Category);
            e.HasIndex(x => new { x.CreatedUtc, x.UpdatedUtc });
        });

        // AuditLog
        b.Entity<AuditLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Action).IsRequired().HasMaxLength(64);
            e.Property(x => x.EntityType).IsRequired().HasMaxLength(64);
            e.Property(x => x.EntityId).IsRequired().HasMaxLength(64);
            e.Property(x => x.Metadata).HasColumnType("TEXT"); // SQLite JSON as TEXT
            e.HasIndex(x => x.TimestampUtc);
            e.HasIndex(x => x.UserId);
        });

        // Setting
        b.Entity<Setting>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Key).IsUnique();
            e.Property(x => x.Key).IsRequired().HasMaxLength(128);
            e.Property(x => x.Value).HasMaxLength(4000);
        });
    }
}
