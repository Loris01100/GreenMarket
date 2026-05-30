using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using GreenMarket.Application.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenMarket.API.Repositories;

public class StockRepository : IStockRepository
{
    private readonly GreenMarketDbContext _context;

    public StockRepository(GreenMarketDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Stock>> GetAllAsync()
    {
        return await _context.Stocks
            .Include(s => s.Produit)
            .ToListAsync();
    }

    public async Task<Stock?> GetByIdAsync(int id)
    {
        return await _context.Stocks
            .Include(s => s.Produit)
            .FirstOrDefaultAsync(s => s.StockId == id);
    }

    public async Task AddAsync(Stock stock)
    {
        await _context.Stocks.AddAsync(stock);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Stock stock)
    {
        _context.Entry(stock).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var stock = await _context.Stocks.FindAsync(id);
        if (stock != null)
        {
            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();
        }
    }
}
