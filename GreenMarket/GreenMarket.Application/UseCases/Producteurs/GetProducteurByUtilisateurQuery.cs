using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Producteurs;

public record GetProducteurByUtilisateurQuery(Guid UtilisateurId) : IRequest<Producteur?>;

public class GetProducteurByUtilisateurQueryHandler : IRequestHandler<GetProducteurByUtilisateurQuery, Producteur?>
{
    private readonly IProducteurRepository _producteurRepository;

    public GetProducteurByUtilisateurQueryHandler(IProducteurRepository producteurRepository)
        => _producteurRepository = producteurRepository;

    public Task<Producteur?> Handle(GetProducteurByUtilisateurQuery request, CancellationToken cancellationToken)
        => _producteurRepository.GetByUtilisateurIdAsync(request.UtilisateurId);
}
