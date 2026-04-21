using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GreenMarket.API.Repositories;

public class CommandeRepository : ICommandeRepository
{
    private readonly GreenMarketDbContext _context;

    public CommandeRepository(GreenMarketDbContext context)
        => _context = context;

    public async Task<Commande?> GetByIdAsync(int id)
        => await _context.Commandes
                         .Include(c => c.LignesCommande)
                             .ThenInclude(l => l.Produit)
                         .Include(c => c.LignesCommande)
                             .ThenInclude(l => l.Producteur)
                         .FirstOrDefaultAsync(c => c.CommandeId == id);

    public async Task<IEnumerable<Commande>> GetByUtilisateurIdAsync(Guid utilisateurId)
        => await _context.Commandes
                         .Include(c => c.LignesCommande)
                         .Where(c => c.UtilisateurId == utilisateurId)
                         .OrderByDescending(c => c.DateCommande)
                         .ToListAsync();

    public async Task<IEnumerable<Commande>> GetByProducteurIdAsync(int producteurId)
        => await _context.Commandes
                         .Include(c => c.LignesCommande)
                         .Where(c => c.LignesCommande.Any(l => l.ProducteurId == producteurId))
                         .OrderByDescending(c => c.DateCommande)
                         .ToListAsync();

    public async Task AddAsync(Commande commande)
    {
        await _context.Commandes.AddAsync(commande);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Commande commande)
    {
        _context.Commandes.Update(commande);
        await _context.SaveChangesAsync();
    }
}