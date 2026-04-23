using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Producteurs;

public record GetProducteurByIdQuery(int ProducteurId) : IRequest<Producteur?>;

public class GetProducteurByIdQueryHandler : IRequestHandler<GetProducteurByIdQuery, Producteur?>
{
    private readonly IProducteurRepository _producteurRepository;

    public GetProducteurByIdQueryHandler(IProducteurRepository producteurRepository)
        => _producteurRepository = producteurRepository;

    public Task<Producteur?> Handle(GetProducteurByIdQuery request, CancellationToken cancellationToken)
        => _producteurRepository.GetByIdAsync(request.ProducteurId);
}
