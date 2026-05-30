using GreenMarket.Application.UseCases.Produits;
using GreenMarket.Domain.Interfaces;
using GreenMarket.Shared.DTOs.Stocks;
using MediatR;

namespace GreenMarket.Application.UseCases.Stocks;

/// <summary>
/// F6 — Mise à jour de la quantité en stock d'un produit par son producteur.
/// La quantité doit être valide (>= 0) et le produit doit appartenir au producteur (F5.4).
/// </summary>
public record UpdateStockCommand(
    int StockId,
    Guid UtilisateurId,
    bool EstAdmin,
    int QuantiteDisponible,
    int? SeuilAlerte = null) : IRequest<StockDto>;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, StockDto>
{
    private readonly IStockRepository _stockRepository;
    private readonly IProduitRepository _produitRepository;
    private readonly IProducteurRepository _producteurRepository;

    public UpdateStockCommandHandler(
        IStockRepository stockRepository,
        IProduitRepository produitRepository,
        IProducteurRepository producteurRepository)
    {
        _stockRepository = stockRepository;
        _produitRepository = produitRepository;
        _producteurRepository = producteurRepository;
    }

    public async Task<StockDto> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        // F6.6 — quantité invalide : la mise à jour n'est pas prise en compte.
        if (request.QuantiteDisponible < 0)
            throw new ArgumentException("La quantité en stock ne peut pas être négative.");

        if (request.SeuilAlerte is < 0)
            throw new ArgumentException("Le seuil d'alerte ne peut pas être négatif.");

        var stock = await _stockRepository.GetByIdAsync(request.StockId)
            ?? throw new KeyNotFoundException($"Stock {request.StockId} introuvable.");

        var produit = await _produitRepository.GetByIdAsync(stock.ProduitId)
            ?? throw new KeyNotFoundException($"Produit {stock.ProduitId} introuvable.");

        await ProduitOwnership.EnsureCanManage(
            produit.ProducteurId, request.UtilisateurId, request.EstAdmin, _producteurRepository);

        stock.QuantiteDisponible = request.QuantiteDisponible;
        if (request.SeuilAlerte is not null)
            stock.SeuilAlerte = request.SeuilAlerte.Value;

        await _stockRepository.UpdateAsync(stock);
        return stock.ToDto();
    }
}
