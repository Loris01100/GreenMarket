using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using GreenMarket.Application.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenMarket.API.Repositories;

public class ProduitRepository
{
    private readonly GreenMarketDbContext _context;

    public ProduitRepository(GreenMarketDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Produit>> GetAllAsync()
    {
        return await _context.Produits
            .Include(p => p.Categorie)
            .ToListAsync();
    }

    public async Task<Produit?> GetByIdAsync(int id)
    {
        return await _context.Produits
            .Include(p => p.Categorie)
            .FirstOrDefaultAsync(p => p.CategorieId == id);
    }

    public async Task AddAsync(Produit produit)
    {
        await _context.Produits.AddAsync(produit);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Produit produit)
    {
        _context.Entry(produit).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var produit = await _context.Produits.FindAsync(id);
        if (produit != null)
        {
            _context.Produits.Remove(produit);
            await _context.SaveChangesAsync();
        }
    }
}
