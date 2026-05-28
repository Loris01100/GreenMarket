namespace GreenMarket.Shared.DTOs.Stocks;

public record StockDto(
    int StockId,
    int ProduitId,
    int QuantiteDisponible,
    int SeuilAlerte
);
