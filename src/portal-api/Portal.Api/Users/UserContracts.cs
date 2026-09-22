namespace Portal.Api.Users;

public sealed record SearchUsersRequest(string? Query, Guid? OrganizationId, int Page = 1, int PageSize = 25);

public sealed record InviteUserRequest(string Email, string DisplayName, Guid OrganizationId);

public sealed record UserResponse(Guid Id, string Email, string DisplayName, Guid? OrganizationId);

public sealed record UserSynchronizationResponse(Guid UserId, string Status, DateTime? LastAttemptUtc, string? ErrorCode);