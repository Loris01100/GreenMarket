using GreenMarket.Application.Data;
using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GreenMarket.API.Repositories;

public class UtilisateurRepository : IUtilisateurRepository
{
    private readonly GreenMarketDbContext _context;

    public UtilisateurRepository(GreenMarketDbContext context) => _context = context;

    public async Task<Utilisateur?> GetByIdAsync(Guid keycloakId)
        => await _context.Utilisateurs
                         .Include(u => u.Producteur)
                         .FirstOrDefaultAsync(u => u.KeycloakId == keycloakId);

    public async Task<IReadOnlyList<Utilisateur>> GetAllAsync()
        => await _context.Utilisateurs.AsNoTracking().ToListAsync();

    public async Task AddAsync(Utilisateur utilisateur)
    {
        await _context.Utilisateurs.AddAsync(utilisateur);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Utilisateur utilisateur)
    {
        _context.Utilisateurs.Update(utilisateur);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid keycloakId)
    {
        var utilisateur = await _context.Utilisateurs.FindAsync(keycloakId);
        if (utilisateur is not null)
        {
            _context.Utilisateurs.Remove(utilisateur);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid keycloakId)
        => await _context.Utilisateurs.AnyAsync(u => u.KeycloakId == keycloakId);
}
