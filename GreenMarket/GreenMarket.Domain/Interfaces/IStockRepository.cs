using GreenMarket.Domain.Entities;

namespace GreenMarket.Domain.Interfaces;

public interface IStockRepository
{
    Task<IEnumerable<Stock>> GetAllAsync();
    Task<Stock?> GetByIdAsync(int id);
    Task<Stock?> GetByProduitIdAsync(int produitId);
    Task AddAsync(Stock stock);
    Task UpdateAsync(Stock stock);
    Task DeleteAsync(int id);
}
