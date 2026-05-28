using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Stocks;

public record GetStockByProduitQuery(int ProduitId) : IRequest<Stock?>;

public class GetStockByProduitQueryHandler : IRequestHandler<GetStockByProduitQuery, Stock?>
{
    private readonly IStockRepository _stockRepository;

    public GetStockByProduitQueryHandler(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<Stock?> Handle(GetStockByProduitQuery request, CancellationToken cancellationToken)
    {
        var stocks = await _stockRepository.GetAllAsync();
        return stocks.FirstOrDefault(s => s.ProduitId == request.ProduitId);
    }
}
