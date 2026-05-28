using GreenMarket.Domain.Entities;

namespace GreenMarket.Domain.Interfaces;
public interface IProduitRepository
{
    Task<IEnumerable<Produit>> GetAllAsync();
    Task<Produit?> GetByIdAsync(int id);
    Task AddAsync(Produit produit);
    Task UpdateAsync(Produit produit);
    Task DeleteAsync(int id);
}
