using GreenMarket.Domain.Entities;

namespace GreenMarket.Domain.Interfaces;

public interface ICommandeRepository
{
    Task<Commande?> GetByIdAsync(int id);
    Task<IEnumerable<Commande>> GetByUtilisateurIdAsync(Guid utilisateurId);
    Task<IEnumerable<Commande>> GetByProducteurIdAsync(int producteurId);
    Task AddAsync(Commande commande);
    Task UpdateAsync(Commande commande);
}