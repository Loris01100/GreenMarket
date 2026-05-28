using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Commandes;

public record ValiderPaiementCommand(int CommandeId, string PaymentIntentId) : IRequest;

public class ValiderPaiementCommandHandler : IRequestHandler<ValiderPaiementCommand>
{
    private readonly ICommandeRepository _commandeRepo;
    private readonly IPaiementService _paiementService;

    public ValiderPaiementCommandHandler(
        ICommandeRepository commandeRepo,
        IPaiementService paiementService)
    {
        _commandeRepo    = commandeRepo;
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
    }
}