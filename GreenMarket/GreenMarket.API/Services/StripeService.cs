using GreenMarket.API.Options;
using GreenMarket.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Stripe;

namespace GreenMarket.API.Services;

public class StripeService : IPaiementService
{
    private readonly StripeOptions _options;
    private readonly ILogger<StripeService> _logger;

    public StripeService(IOptions<StripeOptions> options, ILogger<StripeService> logger)
    {
        _options = options.Value;
        _logger  = logger;
        StripeConfiguration.ApiKey = _options.SecretKey;
    }

    public async Task<string> CreerPaymentIntentAsync(
        decimal montant,
        string devise = "eur",
        int? commandeId = null)
    {
        var service = new PaymentIntentService();

        var options = new PaymentIntentCreateOptions
        {
            Amount   = ConvertirEnCentimes(montant),
            Currency = devise,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            },
            Metadata = commandeId.HasValue
                ? new Dictionary<string, string> { ["commandeId"] = commandeId.Value.ToString() }
                : null
        };

        var intent = await service.CreateAsync(options);
        return intent.ClientSecret;
    }

    public async Task<bool> ConfirmerPaiementAsync(string paymentIntentId, decimal montantAttendu)
    {
        var service = new PaymentIntentService();
        var intent  = await service.GetAsync(paymentIntentId);

        bool statut  = intent.Status == "succeeded";
        bool montant = intent.Amount == ConvertirEnCentimes(montantAttendu);

        if (!statut)
            _logger.LogWarning("PaymentIntent {Id} : statut inattendu '{Statut}'", paymentIntentId, intent.Status);

        if (!montant)
            _logger.LogWarning("PaymentIntent {Id} : montant attendu {Attendu} ≠ reçu {Recu}",
                paymentIntentId, montantAttendu, intent.Amount / 100m);

        return statut && montant;
    }

    private static long ConvertirEnCentimes(decimal montant) => (long)Math.Round(montant * 100);
}