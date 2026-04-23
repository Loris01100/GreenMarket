using GreenMarket.Application.DTOs;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Commandes;

public record GetCommandesUtilisateurQuery(Guid UtilisateurId) : IRequest<IEnumerable<CommandeDto>>;

public class GetCommandesUtilisateurQueryHandler
    : IRequestHandler<GetCommandesUtilisateurQuery, IEnumerable<CommandeDto>>
{
    private readonly ICommandeRepository _commandeRepository;

    public GetCommandesUtilisateurQueryHandler(ICommandeRepository commandeRepository)
        => _commandeRepository = commandeRepository;

    public async Task<IEnumerable<CommandeDto>> Handle(
        GetCommandesUtilisateurQuery request, CancellationToken cancellationToken)
    {
        var commandes = await _commandeRepository.GetByUtilisateurIdAsync(request.UtilisateurId);
        return commandes.Select(CommandeMappingExtensions.ToDto);
    }
}