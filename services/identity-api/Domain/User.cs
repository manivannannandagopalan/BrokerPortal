namespace BrokerPortal.IdentityApi.Domain;

public sealed record User(
    Guid Id,
    string Name,
    string Email,
    Guid BrokerId,
    string Role,
    UserStatus Status,
    DateTimeOffset? LastSignIn);

public enum UserStatus
{
    Active,
    Pending,
    Suspended
}
