using GreenMarket.API.Core.Models;
using GreenMarket.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GreenMarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProduitsController : ControllerBase
{
    private readonly ProduitRepository _repository;

    public ProduitsController(ProduitRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produit>>> GetAll()
    {
        var produits = await _repository.GetAllAsync();
        return Ok(produits);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Produit>> GetById(int id)
    {
        var produit = await _repository.GetByIdAsync(id);
        if (produit == null)
        {
            return NotFound();
        }
        return Ok(produit);
    }

    [HttpPost]
    public async Task<ActionResult<Produit>> Create(Produit produit)
    {
        await _repository.AddAsync(produit);
        return CreatedAtAction(nameof(GetById), new { id = produit.Id }, produit);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Produit produit)
    {
        if (id != produit.Id)
        {
            return BadRequest();
        }

        await _repository.UpdateAsync(produit);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var produit = await _repository.GetByIdAsync(id);
        if (produit == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
