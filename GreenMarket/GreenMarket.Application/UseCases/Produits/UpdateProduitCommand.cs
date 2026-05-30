using GreenMarket.Domain.Interfaces;
using GreenMarket.Shared.DTOs.Produits;
using MediatR;

namespace GreenMarket.Application.UseCases.Produits;

/// <summary>
/// F5 — Modification d'un produit existant. Le producteur ne peut modifier que
/// les produits qu'il a référencés (F5.4) ; l'administrateur n'est pas restreint.
/// </summary>
public record UpdateProduitCommand(
    int Id,
    Guid UtilisateurId,
    bool EstAdmin,
    ProduitCreateDto Dto) : IRequest<ProduitDto>;

public class UpdateProduitCommandHandler : IRequestHandler<UpdateProduitCommand, ProduitDto>
{
    private readonly IProduitRepository _produitRepository;
    private readonly IProducteurRepository _producteurRepository;

    public UpdateProduitCommandHandler(
        IProduitRepository produitRepository,
        IProducteurRepository producteurRepository)
    {
        _produitRepository = produitRepository;
        _producteurRepository = producteurRepository;
    }

    public async Task<ProduitDto> Handle(UpdateProduitCommand request, CancellationToken cancellationToken)
    {
        var produit = await _produitRepository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Produit {request.Id} introuvable.");

        await ProduitOwnership.EnsureCanManage(
            produit.ProducteurId, request.UtilisateurId, request.EstAdmin, _producteurRepository);

        var dto = request.Dto;
        produit.Nom = dto.Nom.Trim();
        produit.Description = dto.Description;
        produit.PrixUnitaire = dto.PrixUnitaire;
        produit.CategorieId = dto.CategorieId;
        produit.ScoreEnvironnemental = dto.ScoreEnvironnemental;
        produit.Tracabilite = dto.Tracabilite;

        await _produitRepository.UpdateAsync(produit);

        var maj = await _produitRepository.GetByIdAsync(produit.ProduitId);
        return maj!.ToDto();
    }
}
