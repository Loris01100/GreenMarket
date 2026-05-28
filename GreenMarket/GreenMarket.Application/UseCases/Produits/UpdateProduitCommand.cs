using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Produits;

public record UpdateProduitCommand(
    int Id,
    string Nom,
    string Description,
    decimal Prix,
    int CategorieId) : IRequest<Produit>;

public class UpdateProduitCommandHandler : IRequestHandler<UpdateProduitCommand, Produit>
{
    private readonly IProduitRepository _produitRepository;

    public UpdateProduitCommandHandler(IProduitRepository produitRepository)
    {
        _produitRepository = produitRepository;
    }

    public async Task<Produit> Handle(UpdateProduitCommand request, CancellationToken cancellationToken)
    {
        var produit = await _produitRepository.GetByIdAsync(request.Id);

        if (produit == null)
        {
            throw new KeyNotFoundException($"Produit with ID {request.Id} not found.");
        }

        produit.Nom = request.Nom;
        produit.Description = request.Description;
        produit.PrixUnitaire = request.Prix;
        produit.CategorieId = request.CategorieId;

        await _produitRepository.UpdateAsync(produit);
        return produit;
    }
}
