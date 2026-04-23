namespace GreenMarket.Application.Interfaces;

public interface IPaiementService
{
    Task<string> CreerPaymentIntentAsync(decimal montant, string devise = "eur");
    Task<bool> ConfirmerPaiementAsync(string paymentIntentId, decimal montantAttendu);
}