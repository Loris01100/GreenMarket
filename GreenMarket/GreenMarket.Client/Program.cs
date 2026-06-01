using GreenMarket.Client;
using GreenMarket.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = "http://localhost:5210";

// Client authentifié : attache le jeton Keycloak (appels protégés — espace producteur, etc.).
builder.Services.AddHttpClient("GreenMarket.Authorized", client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
    })
    .AddHttpMessageHandler(sp =>
    {
        var handler = sp.GetRequiredService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                authorizedUrls: [apiBaseUrl],
                scopes: ["openid", "profile"]
            );
        return handler;
    });

// Client anonyme : utilisé par défaut pour la consultation publique du catalogue (F2),
// accessible sans authentification. N'exige aucun jeton.
builder.Services.AddHttpClient("GreenMarket.Public", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("GreenMarket.Public"));

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Keycloak", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.UserOptions.RoleClaim = "roles";
});

builder.Services.AddFluentUIComponents();

builder.Services.AddScoped<CartService>();

await builder.Build().RunAsync();
