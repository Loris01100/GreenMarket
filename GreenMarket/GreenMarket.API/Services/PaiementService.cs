using GreenMarket.API.Options;
using GreenMarket.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Stripe;

namespace GreenMarket.API.Services;

public class PaiementService : IPaiementService
{
    private readonly ICommandeRepository _commandeRepo;

    public PaiementService(ICommandeRepository commandeRepo, IOptions<StripeOptions> options)
    {
        _commandeRepo = commandeRepo;
        StripeConfiguration.ApiKey = options.Value.SecretKey;
    }

    public async Task<string> CreerPaymentIntentAsync(
        decimal montant, string devise = "eur", int? commandeId = null)
    {
        long montantCentimes = (long)(montant * 100);

        if (montantCentimes <= 0)
            throw new InvalidOperationException("Le montant doit être supérieur à 0.");

        var options = new PaymentIntentCreateOptions
        {
            Amount             = montantCentimes,
            Currency           = devise,
            PaymentMethodTypes = ["card"],
            Metadata           = commandeId.HasValue
                ? new Dictionary<string, string>
                  {
                      ["commandeId"] = commandeId.Value.ToString()
                  }
                : null
        };

        var service = new PaymentIntentService();
        var intent  = await service.CreateAsync(options);

        if (commandeId.HasValue)
        {
            var commande = await _commandeRepo.GetByIdAsync(commandeId.Value);
            if (commande is not null)
            {
                commande.StripePaymentIntentId = intent.Id;
                await _commandeRepo.UpdateAsync(commande);
            }
        }

        return intent.ClientSecret;
    }

    public async Task<bool> ConfirmerPaiementAsync(
        string paymentIntentId, decimal montantAttendu)
    {
        var service = new PaymentIntentService();
        var intent  = await service.GetAsync(paymentIntentId);

        if (intent.Status != "succeeded")
            return false;

        long montantAttenduCentimes = (long)(montantAttendu * 100);
        if (intent.Amount != montantAttenduCentimes)
            throw new InvalidOperationException(
                $"Montant incohérent : attendu {montantAttenduCentimes}, reçu {intent.Amount}.");

        if (intent.Metadata.TryGetValue("commandeId", out var idStr)
            && int.TryParse(idStr, out var commandeId))
        {
            var commande = await _commandeRepo.GetByIdAsync(commandeId);
            if (commande is not null && commande.StatutPaiement != "paye")
            {
                commande.StatutPaiement = "paye";
                await _commandeRepo.UpdateAsync(commande);
            }
        }

        return true;
    }
}