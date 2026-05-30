using System.ComponentModel.DataAnnotations;

namespace GreenMarket.Shared.DTOs.Stocks;

/// <summary>
/// Mise à jour d'un stock par le producteur (F6).
/// </summary>
public record StockUpdateDto(
    [Range(0, int.MaxValue, ErrorMessage = "La quantité disponible doit être positive.")]
    int QuantiteDisponible,

    [Range(0, int.MaxValue, ErrorMessage = "Le seuil d'alerte doit être positif.")]
    int? SeuilAlerte = null
);
