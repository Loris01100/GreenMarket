using GreenMarket.Domain.Entities;

namespace GreenMarket.Domain.Interfaces;

public interface IProducteurRepository
{
    Task<Producteur?> GetByIdAsync(int producteurId);
    Task<Producteur?> GetByUtilisateurIdAsync(Guid utilisateurId);
    Task<IReadOnlyList<Producteur>> GetAllAsync();
    Task AddAsync(Producteur producteur);
    Task UpdateAsync(Producteur producteur);
    Task DeleteAsync(int producteurId);
    Task<bool> ExistsAsync(int producteurId);
}
