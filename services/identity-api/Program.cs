using BrokerPortal.IdentityApi.Application;
using BrokerPortal.IdentityApi.Domain;
using BrokerPortal.IdentityApi.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IDuckCreekUserGateway, MockDuckCreekUserGateway>();
builder.Services.AddSingleton<UserStore>();
var auth0Domain = builder.Configuration["Auth0:Domain"];
var auth0Audience = builder.Configuration["Auth0:Audience"];
if (!string.IsNullOrWhiteSpace(auth0Domain) && !string.IsNullOrWhiteSpace(auth0Audience))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.Authority = $"https://{auth0Domain}/";
        options.Audience = auth0Audience;
        options.RequireHttpsMetadata = true;
    });
}
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("users.read", policy => policy.RequireAuthenticatedUser().RequireClaim("permissions", "users.read"));
    options.AddPolicy("users.invite", policy => policy.RequireAuthenticatedUser().RequireClaim("permissions", "users.invite"));
    options.AddPolicy("users.status.write", policy => policy.RequireAuthenticatedUser().RequireClaim("permissions", "users.status.write"));
});

var app = builder.Build();
if (!string.IsNullOrWhiteSpace(auth0Domain) && !string.IsNullOrWhiteSpace(auth0Audience))
{
    app.UseAuthentication();
}
app.UseAuthorization();
app.Use(async (context, next) =>
{
    const string header = "X-Correlation-ID";
    var correlationId = context.Request.Headers[header].FirstOrDefault();
    if (string.IsNullOrWhiteSpace(correlationId)) correlationId = Guid.NewGuid().ToString("N");
    context.Response.Headers[header] = correlationId;
    using (app.Logger.BeginScope(new Dictionary<string, object> { [header] = correlationId })) await next();
});
app.MapHealthChecks("/health").AllowAnonymous();

app.MapGet("/api/users", (string? search, string? status, Guid? brokerId, UserStore store) =>
{
    var users = store.All.Where(user =>
        (string.IsNullOrWhiteSpace(search) || $"{user.Name} {user.Email}".Contains(search, StringComparison.OrdinalIgnoreCase)) &&
        (string.IsNullOrWhiteSpace(status) || user.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase)) &&
        (!brokerId.HasValue || user.BrokerId == brokerId.Value));
    return Results.Ok(users.Select(UserResponse.From));
}).RequireAuthorization("users.read");

app.MapPost("/api/users/invitations", async (InvitationRequest request, UserStore store, IDuckCreekUserGateway gateway, CancellationToken cancellationToken) =>
{
    if (store.All.Any(user => user.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) && user.Status == UserStatus.Pending)) return Results.Conflict();
    var user = new User(Guid.NewGuid(), request.Email.Split('@')[0], request.Email, request.BrokerId, request.Role, UserStatus.Pending, null);
    store.Add(user);
    await gateway.ProvisionUserAsync(user, cancellationToken);
    return Results.Accepted($"/api/users/{user.Id}", UserResponse.From(user));
}).RequireAuthorization("users.invite");

app.MapPatch("/api/users/{userId:guid}/status", (Guid userId, StatusChangeRequest request, UserStore store) =>
{
    var user = store.Find(userId);
    if (user is null) return Results.NotFound();
    var updated = user with { Status = request.Status };
    store.Replace(updated);
    return Results.Ok(UserResponse.From(updated));
}).RequireAuthorization("users.status.write");

app.Run();

public sealed class UserStore
{
    private readonly List<User> users = [];
    public IReadOnlyList<User> All => users;
    public void Add(User user) => users.Add(user);
    public User? Find(Guid id) => users.FirstOrDefault(user => user.Id == id);
    public void Replace(User replacement)
    {
        var index = users.FindIndex(user => user.Id == replacement.Id);
        if (index >= 0) users[index] = replacement;
    }
}
