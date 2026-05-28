namespace GreenMarket.Shared.DTOs.Produits;

public record ProduitCreateDto(
    string Nom,
    string? Description,
    decimal PrixUnitaire,
    int ProducteurId,
    int CategorieId
);
