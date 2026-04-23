using System.ComponentModel.DataAnnotations;

namespace GreenMarket.Shared.DTOs;

public record ProducteurCreateDto(
    [Required][MaxLength(200)] string NomProducteur,
    [Required][MaxLength(500)] string Adresse,
    string? Description
);
