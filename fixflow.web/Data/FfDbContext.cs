using fixflow.web.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace fixflow.web.Data;

public class FfDbContext : IdentityDbContext<AppUser>
{
    public FfDbContext(DbContextOptions<FfDbContext> options)
        : base(options)
    {
    }

    // Data tables
    DbSet<FfUserProfile> FfUserProfile { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)       
    {
        base.OnModelCreating(builder);

        // This creates a converter that ensures UTC kind
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        // This applies it to every DateTime property in your entire database
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(utcConverter);
                }
            }
        }

        builder.Entity<FfUserProfile>()                   // PstsUserProfile Primary and Foreign Key defininitions
            .HasKey(a => a.EmployeeId);

        builder.Entity<FfUserProfile>()
            .HasOne(a => a.User)
            .WithOne()
            .HasForeignKey<FfUserProfile>(a => a.EmployeeId);

        builder.Entity<FfUserProfile>()
            .HasOne(a => a.Manager)
            .WithMany()
            .HasForeignKey(a => a.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        
        
 
    }

}
