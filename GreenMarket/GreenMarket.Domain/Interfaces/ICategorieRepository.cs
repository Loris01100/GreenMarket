using GreenMarket.Domain.Entities;

namespace GreenMarket.Domain.Interfaces;
public interface ICategorieRepository
{
    Task<IEnumerable<Categorie>> GetAllAsync();
    Task<Categorie?> GetByIdAsync(int id);
    Task AddAsync(Categorie categorie);
    Task UpdateAsync(Categorie categorie);
    Task DeleteAsync(int id);
}
