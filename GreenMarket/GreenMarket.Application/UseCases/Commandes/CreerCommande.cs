using GreenMarket.Application.DTOs;
using GreenMarket.Application.Interfaces;
using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Commandes;

public record CreerCommandeCommand(Guid UtilisateurId, CommandeCreateDto Dto) : IRequest<CommandeDto>;
public class CreerCommandeHandler : IRequestHandler<CreerCommandeCommand, CommandeDto>
{
    private readonly ICommandeRepository _commandeRepository;
    private readonly IPaiementService _paiementService;

    public CreerCommandeHandler(
        ICommandeRepository commandeRepository,
        IPaiementService paiementService)
    {
        _commandeRepository = commandeRepository;
        _paiementService = paiementService;
    }

     public async Task<CommandeDto> Handle(CreerCommandeCommand request, CancellationToken cancellationToken)
    {
        var lignes = request.Dto.Lignes.Select(l => new LigneCommande
        {
            ProduitId    = l.ProduitId,
            ProducteurId = l.ProducteurId,
            Quantite     = l.Quantite,
            PrixUnitaire = l.PrixUnitaire
        }).ToList();

        var commande = new Commande
        {
            UtilisateurId  = request.UtilisateurId,
            MontantTotal   = lignes.Sum(l => l.PrixUnitaire * l.Quantite),
            StatutPaiement = "en_attente",
            LignesCommande = lignes
        };

        await _commandeRepository.AddAsync(commande);

        return CommandeMappingExtensions.ToDto(commande);
    }
}