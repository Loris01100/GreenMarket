using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Utilisateurs;

public record GetUtilisateurQuery(Guid KeycloakId) : IRequest<UtilisateurResult?>;

public record UtilisateurResult(Guid KeycloakId, DateTimeOffset DateInscription, bool EstProducteur);

public class GetUtilisateurQueryHandler : IRequestHandler<GetUtilisateurQuery, UtilisateurResult?>
{
    private readonly IUtilisateurRepository _utilisateurRepository;

    public GetUtilisateurQueryHandler(IUtilisateurRepository utilisateurRepository)
        => _utilisateurRepository = utilisateurRepository;

    public async Task<UtilisateurResult?> Handle(GetUtilisateurQuery request, CancellationToken cancellationToken)
    {
        var utilisateur = await _utilisateurRepository.GetByIdAsync(request.KeycloakId);
        if (utilisateur is null) return null;

        return new UtilisateurResult(
            utilisateur.KeycloakId,
            utilisateur.DateInscription,
            utilisateur.Producteur is not null
        );
    }
}
