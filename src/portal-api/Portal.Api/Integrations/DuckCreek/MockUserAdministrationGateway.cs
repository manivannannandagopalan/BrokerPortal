namespace Portal.Api.Integrations.DuckCreek;

public sealed class MockUserAdministrationGateway : IUserAdministrationGateway
{
    public Task<ExternalUserSyncResult> SynchronizeUserAsync(
        ExternalUserSyncRequest request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new ExternalUserSyncResult(
            Succeeded: true,
            ExternalReference: $"mock:{request.ExternalSubject}",
            ErrorCode: null));
    }
}