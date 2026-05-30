using GreenMarket.Domain.Interfaces;
using MediatR;

namespace GreenMarket.Application.UseCases.Produits;

/// <summary>
/// F5 — Suppression d'un produit. Soumise au contrôle d'appartenance (F5.4).
/// </summary>
public record DeleteProduitCommand(int Id, Guid UtilisateurId, bool EstAdmin) : IRequest;

public class DeleteProduitCommandHandler : IRequestHandler<DeleteProduitCommand>
{
    private readonly IProduitRepository _produitRepository;
    private readonly IProducteurRepository _producteurRepository;

    public DeleteProduitCommandHandler(
        IProduitRepository produitRepository,
        IProducteurRepository producteurRepository)
    {
        _produitRepository = produitRepository;
        _producteurRepository = producteurRepository;
    }

    public async Task Handle(DeleteProduitCommand request, CancellationToken cancellationToken)
    {
        var produit = await _produitRepository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Produit {request.Id} introuvable.");

        await ProduitOwnership.EnsureCanManage(
            produit.ProducteurId, request.UtilisateurId, request.EstAdmin, _producteurRepository);

        await _produitRepository.DeleteAsync(request.Id);
    }
}
