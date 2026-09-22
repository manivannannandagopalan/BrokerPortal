namespace Portal.Api.Users;

public sealed class PortalUser
{
    public Guid Id { get; set; }
    public required string ExternalSubject { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public Guid? OrganizationId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public byte[] Version { get; set; } = [];
}