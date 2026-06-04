using GreenMarket.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenMarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaiementController : ControllerBase
{
    private readonly IPaiementService _paiement;
    private readonly ICommandeRepository _commandeRepo;
    private readonly ILogger<PaiementController> _logger;

    public PaiementController(
        IPaiementService paiement,
        ICommandeRepository commandeRepo,
        ILogger<PaiementController> logger)
    {
        Console.WriteLine("PAIEMENT CONTROLLER CHARGÉ");
        _paiement     = paiement;
        _commandeRepo = commandeRepo;
        _logger       = logger;
    }

    [HttpPost("intent/{commandeId:int}")]
    [Authorize]
    public async Task<IActionResult> CreerIntent(int commandeId)
    {
        var commande = await _commandeRepo.GetByIdAsync(commandeId);
        if (commande is null)
            return NotFound(new { message = $"Commande {commandeId} introuvable." });

        if (commande.StatutPaiement == "paye")
            return BadRequest(new { message = "Cette commande est déjà payée." });

        try
        {
            var clientSecret = await _paiement.CreerPaymentIntentAsync(
                commande.MontantTotal, "eur", commandeId);

            return Ok(new { clientSecret });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erreur création PaymentIntent commande {CommandeId}.", commandeId);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("confirmer")]
    [Authorize]
    public async Task<IActionResult> Confirmer([FromBody] ConfirmerPaiementDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.PaymentIntentId))
            return BadRequest(new { message = "PaymentIntentId est requis." });

        var commande = await _commandeRepo.GetByIdAsync(dto.CommandeId);
        if (commande is null)
            return NotFound(new { message = $"Commande {dto.CommandeId} introuvable." });

        try
        {
            var succes = await _paiement.ConfirmerPaiementAsync(
                dto.PaymentIntentId, commande.MontantTotal);

            if (!succes)
                return BadRequest(new { message = "Le paiement n'est pas encore confirmé par Stripe." });

            return Ok(new { message = "Paiement confirmé, commande mise à jour." });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Montant incohérent pour {PaymentIntentId}.", dto.PaymentIntentId);
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record ConfirmerPaiementDto(string PaymentIntentId, int CommandeId);