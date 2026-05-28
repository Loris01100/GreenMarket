using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Stocks;

public record UpdateStockCommand(
    int Id,
    int Quantite) : IRequest<Stock>;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, Stock>
{
    private readonly IStockRepository _stockRepository;

    public UpdateStockCommandHandler(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<Stock> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        var stock = await _stockRepository.GetByIdAsync(request.Id);

        if (stock == null)
        {
            throw new KeyNotFoundException($"Stock with ID {request.Id} not found.");
        }

        stock.QuantiteDisponible = request.Quantite;

        await _stockRepository.UpdateAsync(stock);
        return stock;
    }
}
