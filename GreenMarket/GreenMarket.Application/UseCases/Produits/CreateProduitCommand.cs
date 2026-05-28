using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Produits;

public record CreateProduitCommand(
    string Nom,
    string Description,
    decimal Prix,
    int CategorieId) : IRequest<Produit>;

public class CreateProduitCommandHandler : IRequestHandler<CreateProduitCommand, Produit>
{
    private readonly IProduitRepository _produitRepository;

    public CreateProduitCommandHandler(IProduitRepository produitRepository)
    {
        _produitRepository = produitRepository;
    }

    public async Task<Produit> Handle(CreateProduitCommand request, CancellationToken cancellationToken)
    {
        var produit = new Produit
        {
            Nom = request.Nom,
            Description = request.Description,
            PrixUnitaire = request.Prix,
            CategorieId = request.CategorieId
        };

        await _produitRepository.AddAsync(produit);
        return produit;
    }
}
