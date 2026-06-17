using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Commandes;

public record ValiderPaiementCommand(int CommandeId, string PaymentIntentId) : IRequest;

public class ValiderPaiementCommandHandler : IRequestHandler<ValiderPaiementCommand>
{
    private readonly ICommandeRepository _commandeRepo;
    private readonly IStockRepository    _stockRepo;
    private readonly IPaiementService    _paiementService;

    public ValiderPaiementCommandHandler(
        ICommandeRepository commandeRepo,
        IStockRepository stockRepo,
        IPaiementService paiementService)
    {
        _commandeRepo    = commandeRepo;
        _stockRepo       = stockRepo;
        _paiementService = paiementService;
    }

    public async Task Handle(ValiderPaiementCommand request, CancellationToken ct)
    {
        var commande = await _commandeRepo.GetByIdAsync(request.CommandeId)
            ?? throw new KeyNotFoundException($"Commande {request.CommandeId} introuvable.");

        if (commande.StatutPaiement == "paye")
            return;

        var confirme = await _paiementService.ConfirmerPaiementAsync(
            request.PaymentIntentId, commande.MontantTotal);

        if (!confirme)
            throw new InvalidOperationException("Le paiement n'est pas confirmé par Stripe.");

        foreach (var ligne in commande.LignesCommande)
        {
            var stock = await _stockRepo.GetByProduitIdAsync(ligne.ProduitId)
                ?? throw new InvalidOperationException(
                    $"Stock introuvable pour le produit {ligne.ProduitId}.");

            if (stock.QuantiteDisponible < ligne.Quantite)
                throw new InvalidOperationException(
                    $"Stock insuffisant pour le produit {ligne.ProduitId} " +
                    $"(disponible: {stock.QuantiteDisponible}, demandé: {ligne.Quantite}).");

            stock.QuantiteDisponible -= ligne.Quantite;
            await _stockRepo.UpdateAsync(stock);
        }

        commande.StatutPaiement = "paye";
        await _commandeRepo.UpdateAsync(commande);
    }
}