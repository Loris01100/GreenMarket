using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Produits;

public record GetProduitByIdQuery(int Id) : IRequest<Produit?>;

public class GetProduitByIdQueryHandler : IRequestHandler<GetProduitByIdQuery, Produit?>
{
    private readonly IProduitRepository _produitRepository;

    public GetProduitByIdQueryHandler(IProduitRepository produitRepository)
    {
        _produitRepository = produitRepository;
    }

    public async Task<Produit?> Handle(GetProduitByIdQuery request, CancellationToken cancellationToken)
    {
        return await _produitRepository.GetByIdAsync(request.Id);
    }
}
