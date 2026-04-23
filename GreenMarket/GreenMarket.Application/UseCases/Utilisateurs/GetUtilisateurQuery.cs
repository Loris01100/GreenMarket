using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Utilisateurs;

public record GetUtilisateurQuery(Guid KeycloakId) : IRequest<Utilisateur?>;

public class GetUtilisateurQueryHandler : IRequestHandler<GetUtilisateurQuery, Utilisateur?>
{
    private readonly IUtilisateurRepository _utilisateurRepository;

    public GetUtilisateurQueryHandler(IUtilisateurRepository utilisateurRepository)
        => _utilisateurRepository = utilisateurRepository;

    public Task<Utilisateur?> Handle(GetUtilisateurQuery request, CancellationToken cancellationToken)
        => _utilisateurRepository.GetByIdAsync(request.KeycloakId);
}
