using GreenMarket.Domain.Entities;
using GreenMarket.Shared.DTOs;

namespace GreenMarket.Application.Mappings;

public static class ProducteurMappingExtensions
{
    public static ProducteurDto ToDto(Producteur p) => new(
        p.ProducteurId,
        p.UtilisateurId,
        p.NomProducteur,
        p.Adresse,
        p.Description,
        p.DateAdhesion
    );
}
