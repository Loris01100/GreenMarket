using GreenMarket.Application.DTOs;
using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Commandes;

public record CreerCommandeCommand(Guid UtilisateurId, CommandeCreateDto Dto) : IRequest<CommandeDto>;
public class CreerCommandeHandler : IRequestHandler<CreerCommandeCommand, CommandeDto>
{
    private readonly ICommandeRepository _commandeRepository;
    private readonly IUtilisateurRepository _utilisateurRepository;
    private readonly IPaiementService _paiementService;

    public CreerCommandeHandler(
        ICommandeRepository commandeRepository,
        IUtilisateurRepository utilisateurRepository,
        IPaiementService paiementService)
    {
        _commandeRepository = commandeRepository;
        _utilisateurRepository = utilisateurRepository;
        _paiementService = paiementService;
    }

     public async Task<CommandeDto> Handle(CreerCommandeCommand request, CancellationToken cancellationToken)
    {
        // Un acheteur n'est pas forcément déjà présent dans la table utilisateur
        // (jusqu'ici l'insertion n'a lieu qu'à la création d'un producteur).
        // La FK commande.utilisateur_id l'exige : on le provisionne à la volée.
        if (!await _utilisateurRepository.ExistsAsync(request.UtilisateurId))
        {
            await _utilisateurRepository.AddAsync(
                new Utilisateur { KeycloakId = request.UtilisateurId });
        }

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