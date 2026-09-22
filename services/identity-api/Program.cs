using BrokerPortal.IdentityApi.Application;
using BrokerPortal.IdentityApi.Domain;
using BrokerPortal.IdentityApi.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IDuckCreekUserGateway, MockDuckCreekUserGateway>();
var connectionString = builder.Configuration.GetConnectionString("BrokerPortal")
    ?? throw new InvalidOperationException("ConnectionStrings:BrokerPortal is required.");
builder.Services.AddDbContext<PortalDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<UserStore>();
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
else
{
    builder.Services.AddAuthentication("MissingConfiguration")
        .AddScheme<AuthenticationSchemeOptions, MissingConfigurationAuthenticationHandler>("MissingConfiguration", _ => { });
}
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("users.read", policy => policy.RequireAuthenticatedUser().RequireClaim("permissions", "users.read"));
    options.AddPolicy("users.invite", policy => policy.RequireAuthenticatedUser().RequireClaim("permissions", "users.invite"));
    options.AddPolicy("users.status.write", policy => policy.RequireAuthenticatedUser().RequireClaim("permissions", "users.status.write"));
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<PortalDbContext>().Database.EnsureCreatedAsync();
}
app.UseAuthentication();
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

app.MapGet("/api/users", async (string? search, string? status, Guid? brokerId, UserStore store, CancellationToken cancellationToken) =>
{
    var users = await store.SearchAsync(search, status, brokerId, cancellationToken);
    return Results.Ok(users.Select(UserResponse.From));
}).RequireAuthorization("users.read");

app.MapPost("/api/users/invitations", async (InvitationRequest request, UserStore store, IDuckCreekUserGateway gateway, CancellationToken cancellationToken) =>
{
    if (await store.HasPendingInvitationAsync(request.Email, cancellationToken)) return Results.Conflict();
    var user = new User(Guid.NewGuid(), request.Email.Split('@')[0], request.Email, request.BrokerId, request.Role, UserStatus.Pending, null);
    await store.AddAsync(user, cancellationToken);
    await gateway.ProvisionUserAsync(user, cancellationToken);
    return Results.Accepted($"/api/users/{user.Id}", UserResponse.From(user));
}).RequireAuthorization("users.invite");

app.MapPatch("/api/users/{userId:guid}/status", async (Guid userId, StatusChangeRequest request, UserStore store, CancellationToken cancellationToken) =>
{
    var user = await store.FindAsync(userId, cancellationToken);
    if (user is null) return Results.NotFound();
    var updated = user with { Status = request.Status };
    await store.ReplaceAsync(updated, cancellationToken);
    return Results.Ok(UserResponse.From(updated));
}).RequireAuthorization("users.status.write");

app.Run();

public sealed class MissingConfigurationAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public MissingConfigurationAuthenticationHandler(
        Microsoft.Extensions.Options.IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        System.Text.Encodings.Web.UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() =>
        Task.FromResult(AuthenticateResult.Fail("Auth0 is not configured."));
}
