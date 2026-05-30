using GreenMarket.Domain.Interfaces;
using GreenMarket.Shared.DTOs.Produits;
using MediatR;

namespace GreenMarket.Application.UseCases.Produits;

/// <summary>F2 — Affichage de la fiche détaillée d'un produit.</summary>
public record GetProduitByIdQuery(int Id) : IRequest<ProduitDto?>;

public class GetProduitByIdQueryHandler : IRequestHandler<GetProduitByIdQuery, ProduitDto?>
{
    private readonly IProduitRepository _produitRepository;

    public GetProduitByIdQueryHandler(IProduitRepository produitRepository)
    {
        _produitRepository = produitRepository;
    }

    public async Task<ProduitDto?> Handle(GetProduitByIdQuery request, CancellationToken cancellationToken)
    {
        var produit = await _produitRepository.GetByIdAsync(request.Id);
        return produit?.ToDto();
    }
}
