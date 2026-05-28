namespace GreenMarket.Shared.DTOs.Produits;

public record ProduitDto(
    int Id,
    string Nom,
    string Description,
    decimal PrixUnitaire,
    int ProducteurId,
    string NomProducteur
);
