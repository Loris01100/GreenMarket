using GreenMarket.Domain.Interfaces;
using GreenMarket.Shared.DTOs.Stocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenMarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StocksController : ControllerBase
{
    private readonly IStockRepository _repository;

    public StocksController(IStockRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("produit/{produitId}")]
    public async Task<ActionResult<StockDto>> GetByProduit(int produitId)
    {
        var stocks = await _repository.GetAllAsync();
        var stock = stocks.FirstOrDefault(s => s.ProduitId == produitId);
        if (stock is null)
            return NotFound();
        return Ok(new StockDto(stock.StockId, stock.ProduitId, stock.QuantiteDisponible, stock.SeuilAlerte));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Producteur,Admin")]
    public async Task<IActionResult> UpdateQuantite(int id, [FromBody] int nouvelleQuantite)
    {
        var stock = await _repository.GetByIdAsync(id);
        if (stock is null)
            return NotFound();

        stock.QuantiteDisponible = nouvelleQuantite;
        await _repository.UpdateAsync(stock);
        return NoContent();
    }
}
