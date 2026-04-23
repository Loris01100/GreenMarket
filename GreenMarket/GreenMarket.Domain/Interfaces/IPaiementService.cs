namespace GreenMarket.Domain.Interfaces;

public interface IPaiementService
{
    Task<string> CreerPaymentIntentAsync(decimal montant, string devise = "eur", int? commandeId = null);
    Task<bool> ConfirmerPaiementAsync(string paymentIntentId, decimal montantAttendu);
}