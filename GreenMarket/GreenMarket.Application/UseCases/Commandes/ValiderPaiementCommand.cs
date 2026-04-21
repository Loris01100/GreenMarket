using GreenMarket.Application.Interfaces;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Commandes;

public record ValiderPaiementCommand(int CommandeId, string StripePaymentIntentId) : IRequest;

public class ValiderPaiementCommandHandler : IRequestHandler<ValiderPaiementCommand>
{
    private readonly ICommandeRepository _commandeRepository;
    private readonly IPaiementService _paiementService;

    public ValiderPaiementCommandHandler(
        ICommandeRepository commandeRepository,
        IPaiementService paiementService)
    {
        _commandeRepository = commandeRepository;
        _paiementService = paiementService;
    }

    public async Task Handle(ValiderPaiementCommand request, CancellationToken cancellationToken)
    {
        var commande = await _commandeRepository.GetByIdAsync(request.CommandeId)
            ?? throw new KeyNotFoundException($"Commande {request.CommandeId} introuvable.");

        bool ok = await _paiementService.ConfirmerPaiementAsync(
            request.StripePaymentIntentId, commande.MontantTotal);

        if (!ok)
            throw new InvalidOperationException("Paiement Stripe non confirmé.");

        commande.StatutPaiement = "payee";
        await _commandeRepository.UpdateAsync(commande);
    }
}