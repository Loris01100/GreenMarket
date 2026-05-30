using GreenMarket.Domain.Entities;

namespace GreenMarket.Domain.Interfaces;
public interface IProduitRepository
{
    Task<IEnumerable<Produit>> GetAllAsync();

    /// <summary>
    /// Consultation et recherche de produits (F2) : recherche par nom, filtre par
    /// catégorie, tri par prix et restriction aux produits actifs (disponibles).
    /// </summary>
    Task<IEnumerable<Produit>> SearchAsync(
        string? recherche = null,
        int? categorieId = null,
        string? tri = null,
        bool actifsSeulement = false);

    Task<Produit?> GetByIdAsync(int id);
    Task AddAsync(Produit produit);
    Task UpdateAsync(Produit produit);
    Task DeleteAsync(int id);
}
