using System.Security.Claims;
using GreenMarket.Application.DTOs;
using GreenMarket.Domain.Interfaces;
using GreenMarket.Application.UseCases.Categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenMarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CategorieRepository _repository;

    public CategoriesController(CategorieRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categorie>>> GetAll()
    {
        var categories = await _repository.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Categorie>> GetById(int id)
    {
        var categorie = await _repository.GetByIdAsync(id);
        if (categorie == null)
        {
            return NotFound();
        }
        return Ok(categorie);
    }

    [HttpPost]
    public async Task<ActionResult<Categorie>> Create(Categorie categorie)
    {
        await _repository.AddAsync(categorie);
        return CreatedAtAction(nameof(GetById), new { id = categorie.Id }, categorie);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Categorie categorie)
    {
        if (id != categorie.Id)
        {
            return BadRequest();
        }

        await _repository.UpdateAsync(categorie);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var categorie = await _repository.GetByIdAsync(id);
        if (categorie == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
