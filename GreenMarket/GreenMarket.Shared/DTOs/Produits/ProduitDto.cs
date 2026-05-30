namespace GreenMarket.Shared.DTOs.Produits;

public record ProduitDto(
    int ProduitId,
    string Nom,
    string? Description,
    decimal PrixUnitaire,
    bool EstActif,
    int ProducteurId,
    string NomProducteur,
    int CategorieId,
    string CategorieLibelle,
    int? StockDisponible,
    int? ScoreEnvironnemental,
    string? Tracabilite
);
