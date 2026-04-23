using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Producteurs;

public record GetProducteursQuery : IRequest<IReadOnlyList<Producteur>>;

public class GetProducteursQueryHandler : IRequestHandler<GetProducteursQuery, IReadOnlyList<Producteur>>
{
    private readonly IProducteurRepository _producteurRepository;

    public GetProducteursQueryHandler(IProducteurRepository producteurRepository)
        => _producteurRepository = producteurRepository;

    public Task<IReadOnlyList<Producteur>> Handle(GetProducteursQuery request, CancellationToken cancellationToken)
        => _producteurRepository.GetAllAsync();
}
