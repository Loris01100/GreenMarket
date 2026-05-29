using System.Security.Claims;
using GreenMarket.Application.UseCases.Produits;
using GreenMarket.Shared.DTOs.Produits;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenMarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProduitsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProduitsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// F2 — Consultation et recherche du catalogue (accessible sans authentification).
    /// Seuls les produits actifs (disponibles) sont retournés.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProduitDto>>> GetAll(
        [FromQuery] string? recherche = null,
        [FromQuery] int? categorieId = null,
        [FromQuery] string? tri = null)
    {
        var produits = await _mediator.Send(
            new GetProduitsQuery(recherche, categorieId, tri, ActifsSeulement: true));
        return Ok(produits);
    }

    /// <summary>F2 — Fiche détaillée d'un produit.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProduitDto>> GetById(int id)
    {
        var produit = await _mediator.Send(new GetProduitByIdQuery(id));
        return produit is null ? NotFound() : Ok(produit);
    }

    /// <summary>F5 — Création d'un produit par le producteur authentifié.</summary>
    [HttpPost]
    [Authorize(Roles = "Producteur,Admin")]
    public async Task<ActionResult<ProduitDto>> Create(ProduitCreateDto dto)
    {
        try
        {
            var produit = await _mediator.Send(new CreateProduitCommand(CurrentUserId(), dto));
            return CreatedAtAction(nameof(GetById), new { id = produit.ProduitId }, produit);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    /// <summary>F5 — Modification d'un produit (contrôle d'appartenance).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Producteur,Admin")]
    public async Task<ActionResult<ProduitDto>> Update(int id, ProduitCreateDto dto)
    {
        try
        {
            var produit = await _mediator.Send(
                new UpdateProduitCommand(id, CurrentUserId(), IsAdmin(), dto));
            return Ok(produit);
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

    /// <summary>F5 — Suppression d'un produit (contrôle d'appartenance).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Producteur,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _mediator.Send(new DeleteProduitCommand(id, CurrentUserId(), IsAdmin()));
            return NoContent();
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
