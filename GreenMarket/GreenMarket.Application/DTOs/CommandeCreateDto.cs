namespace GreenMarket.Application.DTOs;

public record CommandeCreateDto(
    IEnumerable<LigneCommandeCreateDto> Lignes
);

public record LigneCommandeCreateDto(
    int     ProduitId,
    int     ProducteurId,
    int     Quantite,
    decimal PrixUnitaire
);