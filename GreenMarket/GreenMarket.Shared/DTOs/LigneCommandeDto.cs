using GreenMarket.Domain.Entities;

namespace GreenMarket.Application.DTOs;

public record LigneCommandeDto(
    int     ProduitId,
    int     ProducteurId,
    int     Quantite,
    decimal PrixUnitaire,
    decimal SousTotal
);

public static class LigneCommandeMappingExtensions
{
    public static LigneCommandeDto ToDto(LigneCommande l) => new(
        l.ProduitId,
        l.ProducteurId,
        l.Quantite,
        l.PrixUnitaire,
        l.SousTotal
    );
}