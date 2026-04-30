using System.Security.Claims;
using System.Text.Json;
using GreenMarket.Application.Data;
using GreenMarket.Application.Interfaces;
using GreenMarket.Application.UseCases.Producteurs;
using GreenMarket.API.Endpoints;
using GreenMarket.API.Repositories;
using GreenMarket.API.Services;
using GreenMarket.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// --- Base de données ---
builder.Services.AddDbContext<GreenMarketDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Authentification JWT (Keycloak) ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidAudiences = ["account", builder.Configuration["Keycloak:ClientId"]],
            NameClaimType = "preferred_username"
        };

        // Mapping des rôles Keycloak → claims .NET
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var rolesClaim = context.Principal?.FindFirst("roles");
                if (rolesClaim != null && context.Principal?.Identity is ClaimsIdentity identity)
                {
                    var roles = JsonSerializer.Deserialize<string[]>(rolesClaim.Value);
                    if (roles != null)
                        foreach (var role in roles)
                            identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// --- MediatR ---
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetProducteursQuery).Assembly));

// --- Repositories ---
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IProducteurRepository, ProducteurRepository>();
builder.Services.AddScoped<ICommandeRepository, CommandeRepository>();

// --- Services ---
builder.Services.AddHttpClient<IKeycloakService, KeycloakService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapProducteursEndpoints();
app.MapUtilisateursEndpoints();

app.Run();
