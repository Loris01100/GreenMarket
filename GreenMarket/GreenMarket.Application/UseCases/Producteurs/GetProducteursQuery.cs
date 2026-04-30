using GreenMarket.Domain.Interfaces;
using GreenMarket.Shared.DTOs;
using MediatR;

namespace GreenMarket.Application.UseCases.Producteurs;

public record GetProducteursQuery : IRequest<IEnumerable<ProducteurDto>>;

public class GetProducteursQueryHandler : IRequestHandler<GetProducteursQuery, IEnumerable<ProducteurDto>>
{
    private readonly IProducteurRepository _producteurRepository;

    public GetProducteursQueryHandler(IProducteurRepository producteurRepository)
        => _producteurRepository = producteurRepository;

    public async Task<IEnumerable<ProducteurDto>> Handle(GetProducteursQuery request, CancellationToken cancellationToken)
    {
        var producteurs = await _producteurRepository.GetAllAsync();
        return producteurs.Select(ProducteurMappingExtensions.ToDto);
    }
}
