using GreenMarket.Domain.Interfaces;
using GreenMarket.Shared.DTOs.Produits;
using MediatR;

namespace GreenMarket.Application.UseCases.Produits;

/// <summary>
/// F2 — Consultation et recherche de produits.
/// <paramref name="ActifsSeulement"/> restreint le résultat aux produits disponibles
/// (catalogue public). <paramref name="Tri"/> : prix_asc, prix_desc, nom_asc, nom_desc.
/// </summary>
public record GetProduitsQuery(
    string? Recherche = null,
    int? CategorieId = null,
    string? Tri = null,
    bool ActifsSeulement = false) : IRequest<IEnumerable<ProduitDto>>;

public class GetProduitsQueryHandler : IRequestHandler<GetProduitsQuery, IEnumerable<ProduitDto>>
{
    private readonly IProduitRepository _produitRepository;

    public GetProduitsQueryHandler(IProduitRepository produitRepository)
    {
        _produitRepository = produitRepository;
    }

    public async Task<IEnumerable<ProduitDto>> Handle(GetProduitsQuery request, CancellationToken cancellationToken)
    {
        var produits = await _produitRepository.SearchAsync(
            request.Recherche,
            request.CategorieId,
            request.Tri,
            request.ActifsSeulement);

        return produits.Select(p => p.ToDto());
    }
}
