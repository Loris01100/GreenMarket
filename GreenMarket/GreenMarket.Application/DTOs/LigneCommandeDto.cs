using GreenMarket.Domain.Entities;

namespace GreenMarket.Application.DTOs;

public record LigneCommandeDto(
    int     ProduitId,
    int     ProducteurId,
    string  NomProduit,
    int     Quantite,
    decimal PrixUnitaire,
    decimal SousTotal
);

public static class LigneCommandeMappingExtensions
{
    public static LigneCommandeDto ToDto(LigneCommande l) => new(
        l.ProduitId,
        l.ProducteurId,
        l.Produit?.Nom ?? $"Produit #{l.ProduitId}",
        l.Quantite,
        l.PrixUnitaire,
        l.SousTotal
    );
}