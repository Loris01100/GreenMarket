using GreenMarket.Shared.DTOs.Produits;

namespace GreenMarket.Client.Services;

/// <summary>
/// Panier d'achat côté client (F3). Conservé en mémoire pour la durée de la session
/// du navigateur. Le panier n'est pas persisté côté API tant que la commande n'est
/// pas validée (le paiement sera branché ultérieurement par Dev 3).
/// </summary>
public class CartService
{
    private readonly List<CartItem> _items = new();

    public IReadOnlyList<CartItem> Items => _items;

    /// <summary>Déclenché à chaque modification du panier (utilisé pour le badge du header).</summary>
    public event Action? OnChange;

    public int NombreArticles => _items.Sum(i => i.Quantite);

    public decimal SousTotal => _items.Sum(i => i.PrixUnitaire * i.Quantite);

    public void Ajouter(ProduitDto produit, int quantite = 1)
    {
        if (quantite < 1) quantite = 1;

        var existant = _items.FirstOrDefault(i => i.ProduitId == produit.ProduitId);
        if (existant is not null)
        {
            existant.Quantite += quantite;
        }
        else
        {
            _items.Add(new CartItem
            {
                ProduitId = produit.ProduitId,
                Nom = produit.Nom,
                NomProducteur = produit.NomProducteur,
                ProducteurId = produit.ProducteurId,
                PrixUnitaire = produit.PrixUnitaire,
                StockDisponible = produit.StockDisponible,
                Quantite = quantite
            });
        }
        NotifyChange();
    }

    public void DefinirQuantite(int produitId, int quantite)
    {
        var item = _items.FirstOrDefault(i => i.ProduitId == produitId);
        if (item is null) return;

        if (quantite <= 0)
        {
            _items.Remove(item);
        }
        else
        {
            item.Quantite = quantite;
        }
        NotifyChange();
    }

    public void Supprimer(int produitId)
    {
        var item = _items.FirstOrDefault(i => i.ProduitId == produitId);
        if (item is null) return;
        _items.Remove(item);
        NotifyChange();
    }

    public void Vider()
    {
        _items.Clear();
        NotifyChange();
    }

    private void NotifyChange() => OnChange?.Invoke();
}

public class CartItem
{
    public int ProduitId { get; init; }
    public string Nom { get; init; } = string.Empty;
    public string NomProducteur { get; init; } = string.Empty;
    public int ProducteurId { get; init; }
    public decimal PrixUnitaire { get; init; }
    public int? StockDisponible { get; init; }
    public int Quantite { get; set; }

    public decimal SousTotal => PrixUnitaire * Quantite;
}
