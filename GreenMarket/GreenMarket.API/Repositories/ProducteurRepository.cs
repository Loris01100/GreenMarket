using GreenMarket.Application.Data;
using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GreenMarket.API.Repositories;

public class ProducteurRepository : IProducteurRepository
{
    private readonly GreenMarketDbContext _context;

    public ProducteurRepository(GreenMarketDbContext context) => _context = context;

    public async Task<Producteur?> GetByIdAsync(int producteurId)
        => await _context.Producteurs
                         .Include(p => p.Utilisateur)
                         .FirstOrDefaultAsync(p => p.ProducteurId == producteurId);

    public async Task<Producteur?> GetByUtilisateurIdAsync(Guid utilisateurId)
        => await _context.Producteurs
                         .FirstOrDefaultAsync(p => p.UtilisateurId == utilisateurId);

    public async Task<IReadOnlyList<Producteur>> GetAllAsync()
        => await _context.Producteurs.AsNoTracking().ToListAsync();

    public async Task AddAsync(Producteur producteur)
    {
        await _context.Producteurs.AddAsync(producteur);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Producteur producteur)
    {
        _context.Producteurs.Update(producteur);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int producteurId)
    {
        var producteur = await _context.Producteurs.FindAsync(producteurId);
        if (producteur is not null)
        {
            _context.Producteurs.Remove(producteur);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int producteurId)
        => await _context.Producteurs.AnyAsync(p => p.ProducteurId == producteurId);
}
