using BrokerPortal.IdentityApi.Domain;

namespace BrokerPortal.IdentityApi.Application;

public sealed record InvitationRequest(string Email, Guid BrokerId, string Role);
public sealed record StatusChangeRequest(UserStatus Status, string? Reason);
public sealed record UserResponse(Guid Id, string Name, string Email, Guid BrokerId, string Role, string Status, DateTimeOffset? LastSignIn)
{
    public static UserResponse From(User user) => new(user.Id, user.Name, user.Email, user.BrokerId, user.Role, user.Status.ToString().ToLowerInvariant(), user.LastSignIn);
}
