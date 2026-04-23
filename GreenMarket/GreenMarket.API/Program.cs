using GreenMarket.Application.Data;
using GreenMarket.Domain.Interfaces;
using GreenMarket.API.Repositories;
using GreenMarket.API.Services;
using GreenMarket.API.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<GreenMarketDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(GreenMarket.Application.UseCases.Commandes.CreerCommandeCommand).Assembly));

builder.Services.AddScoped<ICommandeRepository, CommandeRepository>();

builder.Services.Configure<StripeOptions>(
    builder.Configuration.GetSection("Stripe"));
builder.Services.AddScoped<IPaiementService, StripeService>();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();