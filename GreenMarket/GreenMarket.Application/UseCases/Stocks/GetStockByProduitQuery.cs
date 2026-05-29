using GreenMarket.Application.UseCases.Produits;
using GreenMarket.Domain.Interfaces;
using GreenMarket.Shared.DTOs.Stocks;
using MediatR;

namespace GreenMarket.Application.UseCases.Stocks;

/// <summary>F6 — Consultation du stock d'un produit (disponibilité au catalogue).</summary>
public record GetStockByProduitQuery(int ProduitId) : IRequest<StockDto?>;

public class GetStockByProduitQueryHandler : IRequestHandler<GetStockByProduitQuery, StockDto?>
{
    private readonly IStockRepository _stockRepository;

    public GetStockByProduitQueryHandler(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<StockDto?> Handle(GetStockByProduitQuery request, CancellationToken cancellationToken)
    {
        var stocks = await _stockRepository.GetAllAsync();
        var stock = stocks.FirstOrDefault(s => s.ProduitId == request.ProduitId);
        return stock?.ToDto();
    }
}
