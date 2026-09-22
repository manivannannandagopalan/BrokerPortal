using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portal.Api.Persistence;
using Portal.Api.Security;

namespace Portal.Api.Users;

[ApiController]
[Route("api/v1/users")]
public sealed class UsersController(PortalDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.UsersRead)]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> Search(CancellationToken cancellationToken)
    {
        var users = await dbContext.Users
            .AsNoTracking()
            .Where(user => user.IsActive)
            .OrderBy(user => user.DisplayName)
            .Select(user => new UserResponse(user.Id, user.Email, user.DisplayName, user.OrganizationId))
            .ToListAsync(cancellationToken);

        return Ok(users);
    }
}