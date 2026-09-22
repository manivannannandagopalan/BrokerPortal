using BrokerPortal.IdentityApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BrokerPortal.IdentityApi.Infrastructure;

public sealed class PortalDbContext(DbContextOptions<PortalDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Id).HasColumnName("user_id");
            entity.Property(user => user.Name).HasColumnName("display_name").HasMaxLength(200).IsRequired();
            entity.Property(user => user.Email).HasColumnName("email").HasMaxLength(320).IsRequired();
            entity.Property(user => user.BrokerId).HasColumnName("broker_id").IsRequired();
            entity.Property(user => user.Role).HasColumnName("role").HasMaxLength(40).IsRequired();
            entity.Property(user => user.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(user => user.LastSignIn).HasColumnName("last_sign_in");
            entity.HasIndex(user => new { user.Email, user.BrokerId }).IsUnique();
        });
    }
}

public sealed class UserEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public Guid BrokerId { get; set; }
    public string Role { get; set; } = "";
    public UserStatus Status { get; set; }
    public DateTimeOffset? LastSignIn { get; set; }

    public User ToDomain() => new(Id, Name, Email, BrokerId, Role, Status, LastSignIn);
    public static UserEntity FromDomain(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        BrokerId = user.BrokerId,
        Role = user.Role,
        Status = user.Status,
        LastSignIn = user.LastSignIn
    };
}

public sealed class UserStore(PortalDbContext db)
{
    public async Task<IReadOnlyList<User>> SearchAsync(string? search, string? status, Guid? brokerId, CancellationToken cancellationToken)
    {
        var query = db.Users.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(user => user.Name.Contains(search) || user.Email.Contains(search));
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<UserStatus>(status, true, out var parsedStatus)) query = query.Where(user => user.Status == parsedStatus);
        if (brokerId.HasValue) query = query.Where(user => user.BrokerId == brokerId.Value);
        return (await query.ToListAsync(cancellationToken)).Select(user => user.ToDomain()).ToList();
    }

    public Task<bool> HasPendingInvitationAsync(string email, CancellationToken cancellationToken) =>
        db.Users.AnyAsync(user => user.Email == email && user.Status == UserStatus.Pending, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        db.Users.Add(UserEntity.FromDomain(user));
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        (await db.Users.FindAsync([id], cancellationToken))?.ToDomain();

    public async Task ReplaceAsync(User replacement, CancellationToken cancellationToken)
    {
        var entity = await db.Users.FindAsync([replacement.Id], cancellationToken);
        if (entity is null) return;
        entity.Name = replacement.Name;
        entity.Email = replacement.Email;
        entity.BrokerId = replacement.BrokerId;
        entity.Role = replacement.Role;
        entity.Status = replacement.Status;
        entity.LastSignIn = replacement.LastSignIn;
        await db.SaveChangesAsync(cancellationToken);
    }
}
