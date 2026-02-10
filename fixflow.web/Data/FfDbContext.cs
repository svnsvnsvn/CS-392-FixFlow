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
    public DbSet<FfUserProfile> FfUserProfiles { get; set; } = default!;
    public DbSet<FfBuildingDirectory> FfBuildingDirectorys { get; set; } = default!;
    public DbSet<FfExternalNotes> FfExternalNotess { get; set; } = default!;
    public DbSet<FfInternalNotes> FfInternalNotess { get; set; } = default!;
    public DbSet<FfPriorityCodes> FfPriorityCodess { get; set; } = default!;
    public DbSet<FfStatusCodes> FfStatusCodes { get; set; } = default!;
    public DbSet<FfTicketFlow> FfTicketFlows { get; set; } = default!;
    public DbSet<FfTicketRegister> FfTicketRegisters { get; set; } = default!;
    public DbSet<FfTicketTypes> FfTicketTypess { get; set; } = default!;

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

        // *** FfUserProfile ***
        builder.Entity<FfUserProfile>()                   // Primary Key
            .HasKey(a => a.EmployeeId);

        builder.Entity<FfUserProfile>()
            .HasOne(a => a.User)
            .WithOne()
            .HasForeignKey<FfUserProfile>(a => a.EmployeeId);

        builder.Entity<FfUserProfile>()                  // Foreign Key
            .HasOne(a => a.Location)
            .WithMany()
            .HasForeignKey(a => a.LocationCode)
            .OnDelete(DeleteBehavior.Restrict);



        // *** FfBuildingDirectory ***
        builder.Entity<FfBuildingDirectory>()             // Primary Key
            .HasKey(b => b.LocationCode);

        builder.Entity<FfBuildingDirectory>()            // Set Precision.  11cm
            .Property(b => b.LocationLat)
            .HasPrecision(9, 6);

        builder.Entity<FfBuildingDirectory>()            // Set Precision.  11cm
            .Property(b => b.LocationLon)
            .HasPrecision(9, 6);

        builder.Entity<FfBuildingDirectory>()           // Value check to ensure Lat and Lon are in a valid range
            .ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Latitude_Range",
                    "\"LocationLat\" >= -90 AND \"LocationLat\" <= 90"
                );

                t.HasCheckConstraint(
                    "CK_Longitude_Range",
                    "\"LocationLon\" >= -180 AND \"LocationLon\" <= 180"
                );
            });


        // *** FfTicketTypes ***
        builder.Entity<FfTicketTypes>()                   // Primary Key
            .HasKey(c => c.Code);

        builder.Entity<FfTicketTypes>()                  // Ensure Code is incrementing.
            .Property(c => c.Code)
            .UseIdentityColumn();



        // *** FfStatusCodes ***
        builder.Entity<FfStatusCodes>()                   // Primary Key
            .HasKey(d => d.Code);

        builder.Entity<FfStatusCodes>()                  // Ensure Code is incrementing.
            .Property(d => d.Code)
            .UseIdentityColumn();



        // *** FfPriorityCodes ***
        builder.Entity<FfPriorityCodes>()                   // Primary Key
            .HasKey(e => e.Code);

        builder.Entity<FfPriorityCodes>()                  // Ensure Code is incrementing.
            .Property(e => e.Code)
            .UseIdentityColumn();



        // *** FfInternalNotes ***
        builder.Entity<FfInternalNotes>()                   // Primary Key
            .HasKey(f => f.INoteId);

        builder.Entity<FfInternalNotes>()                  // Ensure INoteId is incrementing.
            .Property(f => f.INoteId)
            .UseIdentityColumn();

        builder.Entity<FfInternalNotes>()                  // Foreign Key
            .HasOne(f => f.Ticket)
            .WithMany()
            .HasForeignKey(f => f.TicketId)
            .OnDelete(DeleteBehavior.Restrict);



        // *** FfExternalNotes ***
        builder.Entity<FfExternalNotes>()                   // Primary Key
            .HasKey(g => g.XNoteId);

        builder.Entity<FfExternalNotes>()                  // Ensure XNoteId is incrementing.
            .Property(g => g.XNoteId)
            .UseIdentityColumn();

        builder.Entity<FfExternalNotes>()                  // Foreign Key
            .HasOne(g => g.Ticket)
            .WithMany()
            .HasForeignKey(g => g.TicketId)
            .OnDelete(DeleteBehavior.Restrict);



        // *** FfTicketRegister ***
        builder.Entity<FfTicketRegister>()                   // Primary Key
            .HasKey(h => h.TicketId);

        builder.Entity<FfTicketRegister>()                  // Foreign Key
            .HasOne(h => h.EnteredByUser)
            .WithMany()
            .HasForeignKey(h => h.EnteredBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FfTicketRegister>()                  // Foreign Key
            .HasOne(h => h.RequestedByUser)
            .WithMany()
            .HasForeignKey(h => h.RequestedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FfTicketRegister>()                  // Foreign Key
            .HasOne(h => h.Building)
            .WithMany()
            .HasForeignKey(h => h.Location)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FfTicketRegister>()                  // Foreign Key
            .HasOne(h => h.TicketType)
            .WithMany()
            .HasForeignKey(h => h.TicketTroubleType)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FfTicketRegister>()                  // Foreign Key
            .HasOne(h => h.StatusCode)
            .WithMany()
            .HasForeignKey(h => h.TicketStatus)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FfTicketRegister>()                  // Foreign Key
            .HasOne(h => h.PriorityCode)
            .WithMany()
            .HasForeignKey(h => h.TicketPriority)
            .OnDelete(DeleteBehavior.Restrict);

        // *** FfTicketFlow ***
        builder.Entity<FfTicketFlow>()                   // Primary Key
            .HasKey(i => i.ActionId);

        builder.Entity<FfTicketFlow>()                  // Ensure ActionId is incrementing.
            .Property(i => i.ActionId)
            .UseIdentityColumn();

        builder.Entity<FfTicketFlow>()                  // Foreign Key
            .HasOne(i => i.Ticket)
            .WithMany()
            .HasForeignKey(i => i.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FfTicketFlow>()                  // Foreign Key
            .HasOne(i => i.User)
            .WithMany()
            .HasForeignKey(g => g.NewAssignee)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
