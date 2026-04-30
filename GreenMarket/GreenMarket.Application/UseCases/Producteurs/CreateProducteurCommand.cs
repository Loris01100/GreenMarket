using GreenMarket.Application.Interfaces;
using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using GreenMarket.Shared.DTOs;
using MediatR;

namespace GreenMarket.Application.UseCases.Producteurs;

public record CreateProducteurCommand(Guid UtilisateurId, ProducteurCreateDto Dto) : IRequest<ProducteurDto>;

public class CreateProducteurCommandHandler : IRequestHandler<CreateProducteurCommand, ProducteurDto>
{
    private readonly IProducteurRepository _producteurRepository;
    private readonly IUtilisateurRepository _utilisateurRepository;
    private readonly IKeycloakService _keycloakService;

    public CreateProducteurCommandHandler(
        IProducteurRepository producteurRepository,
        IUtilisateurRepository utilisateurRepository,
        IKeycloakService keycloakService)
    {
        _producteurRepository = producteurRepository;
        _utilisateurRepository = utilisateurRepository;
        _keycloakService = keycloakService;
    }

    public async Task<ProducteurDto> Handle(CreateProducteurCommand request, CancellationToken cancellationToken)
    {
        if (!await _utilisateurRepository.ExistsAsync(request.UtilisateurId))
        {
            await _utilisateurRepository.AddAsync(new Utilisateur { KeycloakId = request.UtilisateurId });
        }

        if (await _producteurRepository.GetByUtilisateurIdAsync(request.UtilisateurId) is not null)
            throw new InvalidOperationException("Cet utilisateur est déjà enregistré comme producteur.");

        var producteur = new Producteur
        {
            UtilisateurId = request.UtilisateurId,
            NomProducteur = request.Dto.NomProducteur,
            Adresse = request.Dto.Adresse,
            Description = request.Dto.Description
        };

        await _producteurRepository.AddAsync(producteur);
        await _keycloakService.AssignerRoleAsync(request.UtilisateurId, "Producteur");

        return ProducteurMappingExtensions.ToDto(producteur);
    }
}
