using GreenMarket.Domain.Entities;
using GreenMarket.Shared.DTOs;

namespace GreenMarket.Application.UseCases.Producteurs;

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
