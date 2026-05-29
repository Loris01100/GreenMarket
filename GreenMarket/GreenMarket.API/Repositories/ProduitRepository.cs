using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using GreenMarket.Application.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenMarket.API.Repositories;

public class ProduitRepository : IProduitRepository
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
            .Include(p => p.Producteur)
            .Include(p => p.Stock)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produit>> SearchAsync(
        string? recherche = null,
        int? categorieId = null,
        string? tri = null,
        bool actifsSeulement = false)
    {
        var query = _context.Produits
            .Include(p => p.Categorie)
            .Include(p => p.Producteur)
            .Include(p => p.Stock)
            .AsNoTracking()
            .AsQueryable();

        if (actifsSeulement)
            query = query.Where(p => p.EstActif);

        if (!string.IsNullOrWhiteSpace(recherche))
        {
            var terme = recherche.Trim().ToLower();
            query = query.Where(p =>
                p.Nom.ToLower().Contains(terme) ||
                (p.Description != null && p.Description.ToLower().Contains(terme)));
        }

        if (categorieId is not null)
            query = query.Where(p => p.CategorieId == categorieId);

        query = tri?.ToLowerInvariant() switch
        {
            "prix_asc" => query.OrderBy(p => p.PrixUnitaire),
            "prix_desc" => query.OrderByDescending(p => p.PrixUnitaire),
            "nom_desc" => query.OrderByDescending(p => p.Nom),
            "nom_asc" => query.OrderBy(p => p.Nom),
            _ => query.OrderByDescending(p => p.DateCreation)
        };

        return await query.ToListAsync();
    }

    public async Task<Produit?> GetByIdAsync(int id)
    {
        return await _context.Produits
            .Include(p => p.Categorie)
            .Include(p => p.Producteur)
            .Include(p => p.Stock)
            .FirstOrDefaultAsync(p => p.ProduitId == id);
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
