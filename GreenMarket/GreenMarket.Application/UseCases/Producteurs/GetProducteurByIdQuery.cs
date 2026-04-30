using GreenMarket.Domain.Interfaces;
using GreenMarket.Shared.DTOs;
using MediatR;

namespace GreenMarket.Application.UseCases.Producteurs;

public record GetProducteurByIdQuery(int ProducteurId) : IRequest<ProducteurDto?>;

public class GetProducteurByIdQueryHandler : IRequestHandler<GetProducteurByIdQuery, ProducteurDto?>
{
    private readonly IProducteurRepository _producteurRepository;

    public GetProducteurByIdQueryHandler(IProducteurRepository producteurRepository)
        => _producteurRepository = producteurRepository;

    public async Task<ProducteurDto?> Handle(GetProducteurByIdQuery request, CancellationToken cancellationToken)
    {
        var producteur = await _producteurRepository.GetByIdAsync(request.ProducteurId);
        return producteur is null ? null : ProducteurMappingExtensions.ToDto(producteur);
    }
}
