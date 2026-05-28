using GreenMarket.API.Core.Models;
using GreenMarket.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GreenMarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StocksController : ControllerBase
{
    private readonly StockRepository _repository;

    public StocksController(StockRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Stock>>> GetAll()
    {
        var stocks = await _repository.GetAllAsync();
        return Ok(stocks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Stock>> GetById(int id)
    {
        var stock = await _repository.GetByIdAsync(id);
        if (stock == null)
        {
            return NotFound();
        }
        return Ok(stock);
    }

    [HttpPost]
    public async Task<ActionResult<Stock>> Create(Stock stock)
    {
        await _repository.AddAsync(stock);
        return CreatedAtAction(nameof(GetById), new { id = stock.Id }, stock);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Stock stock)
    {
        if (id != stock.Id)
        {
            return BadRequest();
        }

        await _repository.UpdateAsync(stock);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var stock = await _repository.GetByIdAsync(id);
        if (stock == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
