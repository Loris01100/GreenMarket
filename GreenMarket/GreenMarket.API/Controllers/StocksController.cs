using System.Security.Claims;
using GreenMarket.Application.UseCases.Stocks;
using GreenMarket.Shared.DTOs.Stocks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenMarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StocksController : ControllerBase
{
    private readonly IMediator _mediator;

    public StocksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>F6 — Disponibilité d'un produit (consultable pour l'affichage catalogue).</summary>
    [HttpGet("produit/{produitId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<StockDto>> GetByProduit(int produitId)
    {
        var stock = await _mediator.Send(new GetStockByProduitQuery(produitId));
        return stock is null ? NotFound() : Ok(stock);
    }

    /// <summary>F6 — Mise à jour du stock par le producteur (contrôle d'appartenance et de validité).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Producteur,Admin")]
    public async Task<ActionResult<StockDto>> UpdateQuantite(int id, StockUpdateDto dto)
    {
        try
        {
            var stock = await _mediator.Send(new UpdateStockCommand(
                id, CurrentUserId(), IsAdmin(), dto.QuantiteDisponible, dto.SeuilAlerte));
            return Ok(stock);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    private Guid CurrentUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private bool IsAdmin() => User.IsInRole("Admin");
}
