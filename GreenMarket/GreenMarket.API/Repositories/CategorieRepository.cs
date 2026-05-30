using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using GreenMarket.Application.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenMarket.API.Repositories;

public class CategorieRepository : ICategorieRepository
{
    private readonly GreenMarketDbContext _context;

    public CategorieRepository(GreenMarketDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Categorie>> GetAllAsync()
    {
        return await _context.Categories
            .ToListAsync();
    }

    public async Task<Categorie?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.CategorieId == id);
    }

    public async Task AddAsync(Categorie categorie)
    {
        await _context.Categories.AddAsync(categorie);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Categorie categorie)
    {
        _context.Entry(categorie).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var categorie = await _context.Categories.FindAsync(id);
        if (categorie != null)
        {
            _context.Categories.Remove(categorie);
            await _context.SaveChangesAsync();
        }
    }
}
