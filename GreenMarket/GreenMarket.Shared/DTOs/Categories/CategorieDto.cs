namespace GreenMarket.Shared.DTOs.Categories;

public record CategorieDto(
    int CategorieId,
    string Libelle,
    string? Description
);
