using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Produits;

public record GetProduitsQuery : IRequest<IEnumerable<Produit>>;

public class GetProduitsQueryHandler : IRequestHandler<GetProduitsQuery, IEnumerable<Produit>>
{
    private readonly IProduitRepository _produitRepository;

    public GetProduitsQueryHandler(IProduitRepository produitRepository)
    {
        _produitRepository = produitRepository;
    }

    public async Task<IEnumerable<Produit>> Handle(GetProduitsQuery request, CancellationToken cancellationToken)
    {
        return await _produitRepository.GetAllAsync();
    }
}
