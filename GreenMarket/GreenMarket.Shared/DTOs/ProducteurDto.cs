namespace GreenMarket.Shared.DTOs;

public record ProducteurDto(
    int ProducteurId,
    Guid UtilisateurId,
    string NomProducteur,
    string Adresse,
    string? Description,
    DateTimeOffset DateAdhesion
);

public record ProducteurCreateDto(
    string NomProducteur,
    string Adresse,
    string? Description
);
