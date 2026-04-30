using System.Security.Claims;
using GreenMarket.Application.UseCases.Producteurs;
using GreenMarket.Application.UseCases.Utilisateurs;
using GreenMarket.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GreenMarket.API.Endpoints;

public static class ProducteursEndpoints
{
    public static void MapProducteursEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/producteurs").RequireAuthorization();

        group.MapGet("/", GetAllProducteurs);
        group.MapGet("/{id:int}", GetProducteurById);
        group.MapPost("/", CreateProducteur).RequireAuthorization(p => p.RequireRole("Acheteur", "Admin"));
    }

    public static void MapUtilisateursEndpoints(this WebApplication app)
    {
        app.MapGet("/api/utilisateurs/me", GetMe).RequireAuthorization();
    }

    private static async Task<IResult> GetAllProducteurs(IMediator mediator)
    {
        var producteurs = await mediator.Send(new GetProducteursQuery());
        return Results.Ok(producteurs);
    }

    private static async Task<IResult> GetProducteurById(int id, IMediator mediator)
    {
        var producteur = await mediator.Send(new GetProducteurByIdQuery(id));
        return producteur is null ? Results.NotFound() : Results.Ok(producteur);
    }

    private static async Task<IResult> CreateProducteur(
        [FromBody] ProducteurCreateDto dto,
        ClaimsPrincipal user,
        IMediator mediator)
    {
        var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var producteur = await mediator.Send(new CreateProducteurCommand(userId, dto));
        return Results.Created($"/api/producteurs/{producteur.ProducteurId}", producteur);
    }

    private static async Task<IResult> GetMe(ClaimsPrincipal user, IMediator mediator)
    {
        var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await mediator.Send(new GetUtilisateurQuery(userId));
        return result is null ? Results.NotFound() : Results.Ok(result);
    }
}
