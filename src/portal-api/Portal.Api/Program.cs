using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Portal.Api.Integrations.DuckCreek;
using Portal.Api.Observability;
using Portal.Api.Persistence;
using Portal.Api.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks().AddDbContextCheck<PortalDbContext>();
builder.Services.AddDbContext<PortalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PortalDatabase")));
builder.Services.AddScoped<IUserAdministrationGateway, MockUserAdministrationGateway>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth0:Authority"];
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.RequireHttpsMetadata = true;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.UsersRead, policy =>
        policy.RequireAuthenticatedUser().RequireClaim("scope", "users:read"));
    options.AddPolicy(AuthorizationPolicies.UsersCreate, policy =>
        policy.RequireAuthenticatedUser().RequireClaim("scope", "users:create"));
});

var app = builder.Build();

app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();

public partial class Program;