using Portal.Api.Security;

namespace Portal.Api.UnitTests;

public sealed class AuthorizationPoliciesTests
{
    [Fact]
    public void UserPoliciesHaveStableNames()
    {
        Assert.Equal("users.read", AuthorizationPolicies.UsersRead);
        Assert.Equal("users.create", AuthorizationPolicies.UsersCreate);
    }
}