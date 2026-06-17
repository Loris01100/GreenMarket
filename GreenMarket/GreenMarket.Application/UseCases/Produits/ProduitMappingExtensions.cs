using GreenMarket.Domain.Entities;
using GreenMarket.Shared.DTOs.Produits;
using GreenMarket.Shared.DTOs.Stocks;

namespace GreenMarket.Application.UseCases.Produits;

public static class ProduitMappingExtensions
{
    public static ProduitDto ToDto(this Produit p) => new(
        p.ProduitId,
        p.Nom,
        p.Description,
        p.ImageUrl,
        p.PrixUnitaire,
        p.EstActif,
        p.ProducteurId,
        p.Producteur?.NomProducteur ?? string.Empty,
        p.CategorieId,
        p.Categorie?.Libelle ?? string.Empty,
        p.Stock?.QuantiteDisponible,
        p.ScoreEnvironnemental,
        p.Tracabilite
    );

    public static StockDto ToDto(this Stock s) => new(
        s.StockId,
        s.ProduitId,
        s.QuantiteDisponible,
        s.SeuilAlerte
    );
}
