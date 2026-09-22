using Microsoft.EntityFrameworkCore;
using Portal.Api.Users;

namespace Portal.Api.Persistence;

public sealed class PortalDbContext(DbContextOptions<PortalDbContext> options) : DbContext(options)
{
    public DbSet<PortalUser> Users => Set<PortalUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PortalUser>(entity =>
        {
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.ExternalSubject).IsUnique();
            entity.Property(user => user.Email).HasMaxLength(320).IsRequired();
            entity.Property(user => user.ExternalSubject).HasMaxLength(200).IsRequired();
            entity.Property(user => user.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(user => user.CreatedUtc).IsRequired();
        });
    }
}