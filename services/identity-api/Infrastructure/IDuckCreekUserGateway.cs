using BrokerPortal.IdentityApi.Domain;

namespace BrokerPortal.IdentityApi.Infrastructure;

public interface IDuckCreekUserGateway
{
    Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken);
    Task ProvisionUserAsync(User user, CancellationToken cancellationToken);
}

public sealed class MockDuckCreekUserGateway : IDuckCreekUserGateway
{
    public Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken) => Task.FromResult(false);

    public Task ProvisionUserAsync(User user, CancellationToken cancellationToken) => Task.CompletedTask;
}
