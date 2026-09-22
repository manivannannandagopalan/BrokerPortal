using Microsoft.AspNetCore.Mvc.Testing;

namespace Portal.Api.IntegrationTests;

public sealed class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpointIsAvailable()
    {
        var response = await client.GetAsync("/health");
        Assert.True(response.IsSuccessStatusCode || (int)response.StatusCode == 503);
    }
}