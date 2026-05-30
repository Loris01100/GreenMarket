using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using GreenMarket.Shared.DTOs.Produits;
using MediatR;

namespace GreenMarket.Application.UseCases.Produits;

/// <summary>
/// F5 — Création d'un produit par un producteur authentifié.
/// Le produit est rattaché au producteur déduit du jeton (<paramref name="UtilisateurId"/>).
/// </summary>
public record CreateProduitCommand(Guid UtilisateurId, ProduitCreateDto Dto) : IRequest<ProduitDto>;

public class CreateProduitCommandHandler : IRequestHandler<CreateProduitCommand, ProduitDto>
{
    private readonly IProduitRepository _produitRepository;
    private readonly IProducteurRepository _producteurRepository;
    private readonly IStockRepository _stockRepository;

    public CreateProduitCommandHandler(
        IProduitRepository produitRepository,
        IProducteurRepository producteurRepository,
        IStockRepository stockRepository)
    {
        _produitRepository = produitRepository;
        _producteurRepository = producteurRepository;
        _stockRepository = stockRepository;
    }

    public async Task<ProduitDto> Handle(CreateProduitCommand request, CancellationToken cancellationToken)
    {
        // F5.4 : seul un producteur référencé peut gérer un catalogue.
        var producteur = await _producteurRepository.GetByUtilisateurIdAsync(request.UtilisateurId)
            ?? throw new UnauthorizedAccessException(
                "Aucun profil producteur n'est associé à ce compte. Enregistrez-vous comme producteur avant de publier un produit.");

        var dto = request.Dto;

        var produit = new Produit
        {
            Nom = dto.Nom.Trim(),
            Description = dto.Description,
            PrixUnitaire = dto.PrixUnitaire,
            ProducteurId = producteur.ProducteurId,
            CategorieId = dto.CategorieId,
            ScoreEnvironnemental = dto.ScoreEnvironnemental,
            Tracabilite = dto.Tracabilite,
            EstActif = true,
            DateCreation = DateTimeOffset.UtcNow
        };

        await _produitRepository.AddAsync(produit);

        // F6 — un produit dispose toujours d'un stock (initialisé à la création).
        await _stockRepository.AddAsync(new Stock
        {
            ProduitId = produit.ProduitId,
            QuantiteDisponible = dto.QuantiteInitiale,
            SeuilAlerte = dto.SeuilAlerte
        });

        var cree = await _produitRepository.GetByIdAsync(produit.ProduitId);
        return cree!.ToDto();
    }
}
