using ItentikaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ItentikaApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<EventRecord> EventRecords => Set<EventRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Incident>(b =>
        {
            b.HasKey(i => i.Id);
            b.Property(i => i.Type).IsRequired();
            b.Property(i => i.Time).IsRequired();
            b.HasMany(i => i.EventRecords)
                .WithOne(e => e.Incident)
                .HasForeignKey(e => e.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EventRecord>(b =>
        {
            b.HasKey(er => er.Id);
            b.Property(er => er.Type).IsRequired();
            b.Property(er => er.Time).IsRequired();
        });
    }
}
