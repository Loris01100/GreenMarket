using System.Security.Claims;
using System.Text.Json;
using GreenMarket.Application.Data;
using GreenMarket.Application.Interfaces;
using GreenMarket.Application.UseCases.Producteurs;
using GreenMarket.Application.UseCases.Commandes;
using GreenMarket.API.Controllers;
using GreenMarket.API.Data;
using GreenMarket.API.Endpoints;
using GreenMarket.API.Services;
using GreenMarket.Domain.Interfaces;
using GreenMarket.API.Repositories;
using GreenMarket.API.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<GreenMarketDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidAudiences =
            [
                "account",
                builder.Configuration["Keycloak:ClientId"]
            ],
            NameClaimType = "preferred_username"
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var rolesClaim = context.Principal?.FindFirst("roles");

                if (rolesClaim != null &&
                    context.Principal?.Identity is ClaimsIdentity identity)
                {
                    var roles =
                        JsonSerializer.Deserialize<string[]>(rolesClaim.Value);

                    if (roles != null)
                    {
                        foreach (var role in roles)
                        {
                            identity.AddClaim(
                                new Claim(ClaimTypes.Role, role));
                        }
                    }
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Origines autorisées : lues depuis la config (Cors:AllowedOrigins en prod),
// fallback sur le client de dev local.
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? ["http://localhost:5200"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(ValiderPaiementCommand).Assembly));

builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IProducteurRepository, ProducteurRepository>();
builder.Services.AddScoped<IProduitRepository, ProduitRepository>();
builder.Services.AddScoped<ICategorieRepository, CategorieRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<ICommandeRepository, CommandeRepository>();

builder.Services.AddHttpClient<IKeycloakService, KeycloakService>();

builder.Services.Configure<StripeOptions>(
    builder.Configuration.GetSection("Stripe"));

builder.Services.AddScoped<IPaiementService, PaiementService>();

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(PaiementController).Assembly)
    .AddControllersAsServices();

// Derrière Cloudflare Tunnel + Caddy : faire confiance aux en-têtes X-Forwarded-*
// pour que l'API connaisse le schéma (https) et l'hôte publics réels.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Proxies internes au réseau Docker : on ne peut pas les énumérer à l'avance.
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Applique automatiquement les migrations EF Core au démarrage (hors dev).
if (!app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<GreenMarketDbContext>();
    db.Database.Migrate();
}

// Jeu de données de test (producteur + produits) pour tester le tunnel de commande.
// Actif en Development ; activable ailleurs via la config "SeedTestData": true.
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("SeedTestData"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<GreenMarketDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await DbSeeder.SeedTestDataAsync(db, logger);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapProducteursEndpoints();
app.MapUtilisateursEndpoints();
app.MapControllers();
app.Run();