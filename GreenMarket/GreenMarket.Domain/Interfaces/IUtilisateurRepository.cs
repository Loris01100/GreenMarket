using GreenMarket.Domain.Entities;

namespace GreenMarket.Domain.Interfaces;

public interface IUtilisateurRepository
{
    Task<Utilisateur?> GetByIdAsync(Guid keycloakId);
    Task<IReadOnlyList<Utilisateur>> GetAllAsync();
    Task AddAsync(Utilisateur utilisateur);
    Task UpdateAsync(Utilisateur utilisateur);
    Task DeleteAsync(Guid keycloakId);
    Task<bool> ExistsAsync(Guid keycloakId);
}
