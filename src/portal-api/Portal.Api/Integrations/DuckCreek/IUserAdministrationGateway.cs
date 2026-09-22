namespace Portal.Api.Integrations.DuckCreek;

public interface IUserAdministrationGateway
{
    Task<ExternalUserSyncResult> SynchronizeUserAsync(
        ExternalUserSyncRequest request,
        CancellationToken cancellationToken);
}

public sealed record ExternalUserSyncRequest(string ExternalSubject, string Email, string DisplayName);

public sealed record ExternalUserSyncResult(bool Succeeded, string? ExternalReference, string? ErrorCode);