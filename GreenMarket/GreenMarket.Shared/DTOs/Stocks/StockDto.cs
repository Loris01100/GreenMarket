namespace GreenMarket.Shared.DTOs.Stocks;

public record StockDto(
    int Id,
    int ProductId,
    double Quantity,
    decimal UnitPrice,
    DateTime? ProductionDate,
    DateTime? ExpirationDate
);
