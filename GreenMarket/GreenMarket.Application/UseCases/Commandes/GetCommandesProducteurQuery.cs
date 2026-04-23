using GreenMarket.Application.DTOs;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Commandes;

public record GetCommandesProducteurQuery(int ProducteurId) : IRequest<IEnumerable<CommandeDto>>;

public class GetCommandesProducteurQueryHandler
    : IRequestHandler<GetCommandesProducteurQuery, IEnumerable<CommandeDto>>
{
    private readonly ICommandeRepository _commandeRepository;

    public GetCommandesProducteurQueryHandler(ICommandeRepository commandeRepository)
        => _commandeRepository = commandeRepository;

    public async Task<IEnumerable<CommandeDto>> Handle(
        GetCommandesProducteurQuery request, CancellationToken cancellationToken)
    {
        var commandes = await _commandeRepository.GetByProducteurIdAsync(request.ProducteurId);
        return commandes.Select(CommandeMappingExtensions.ToDto);
    }
}