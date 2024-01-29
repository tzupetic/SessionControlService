using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SessionControlService.Models;

namespace SessionControlService.Data;

public class SessionDbContext : DbContext
{
    public SessionDbContext(DbContextOptions<SessionDbContext> options)
        : base(options) { }

    public DbSet<Session> Sessions { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Session>()
            .Property(t => t.State)
            .HasConversion(new EnumToStringConverter<SessionState>());
    }
}
