using System.Security.Claims;
using System.Text.Json;
using GreenMarket.Application.Data;
using GreenMarket.Application.Interfaces;
using GreenMarket.Application.Services;
using GreenMarket.Application.UseCases.Producteurs;
using GreenMarket.Application.UseCases.Commandes;
using GreenMarket.API.Controllers;
using GreenMarket.API.Endpoints;
using GreenMarket.API.Services;
using GreenMarket.Domain.Interfaces;
using GreenMarket.API.Repositories;
using GreenMarket.API.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5200")
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

// Configuration du ChatbotService avec HttpClient
builder.Services.AddHttpClient<IChatbotService, ChatbotService>();

builder.Services.AddHttpClient<IKeycloakService, KeycloakService>();

builder.Services.Configure<StripeOptions>(
    builder.Configuration.GetSection("Stripe"));

builder.Services.AddScoped<IPaiementService, PaiementService>();

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(PaiementController).Assembly)
    .AddControllersAsServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapProducteursEndpoints();
app.MapUtilisateursEndpoints();
app.MapChatbotEndpoints();
app.MapControllers();
app.Run();
