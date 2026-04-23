using System.Security.Claims;
using GreenMarket.Application.DTOs;
using GreenMarket.Application.Interfaces;
using GreenMarket.Application.UseCases.Commandes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenMarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommandesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPaiementService _paiementService;

    public CommandesController(IMediator mediator, IPaiementService paiementService)
    {
        _mediator = mediator;
        _paiementService = paiementService;
    }

    [HttpGet("mes-commandes")]
    public async Task<ActionResult<IEnumerable<CommandeDto>>> GetMesCommandes()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _mediator.Send(new GetCommandesUtilisateurQuery(userId)));
    }

    [HttpGet("producteur/{producteurId:int}")]
    [Authorize(Roles = "Producteur,Admin")]
    public async Task<ActionResult<IEnumerable<CommandeDto>>> GetCommandesProducteur(int producteurId)
        => Ok(await _mediator.Send(new GetCommandesProducteurQuery(producteurId)));

    [HttpPost]
    public async Task<ActionResult> CreerCommande([FromBody] CommandeCreateDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var commande = await _mediator.Send(new CreerCommandeCommand(userId, dto));
        var clientSecret = await _paiementService.CreerPaymentIntentAsync(commande.MontantTotal);

        return CreatedAtAction(nameof(GetMesCommandes), new { id = commande.CommandeId },
            new { Commande = commande, StripeClientSecret = clientSecret });
    }

    [HttpPost("{id:int}/valider-paiement")]
    public async Task<IActionResult> ValiderPaiement(int id, [FromBody] ValiderPaiementRequest request)
    {
        await _mediator.Send(new ValiderPaiementCommand(id, request.PaymentIntentId));
        return NoContent();
    }
}

public record ValiderPaiementRequest(string PaymentIntentId);