using GreenMarket.Application.Data;
using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GreenMarket.API.Repositories;

public class ProducteurRepository : IProducteurRepository
{
    private readonly GreenMarketDbContext _context;

    public ProducteurRepository(GreenMarketDbContext context)
        => _context = context;

    public Task<Producteur?> GetByIdAsync(int producteurId)
        => _context.Producteurs
                   .Include(p => p.Utilisateur)
                   .FirstOrDefaultAsync(p => p.ProducteurId == producteurId);

    public Task<Producteur?> GetByUtilisateurIdAsync(Guid utilisateurId)
        => _context.Producteurs
                   .Include(p => p.Utilisateur)
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

    public Task<bool> ExistsAsync(int producteurId)
        => _context.Producteurs.AnyAsync(p => p.ProducteurId == producteurId);
}
