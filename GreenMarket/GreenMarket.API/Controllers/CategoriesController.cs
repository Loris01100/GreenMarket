using GreenMarket.Domain.Interfaces;
using GreenMarket.Shared.DTOs.Categories;
using Microsoft.AspNetCore.Mvc;

namespace GreenMarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategorieRepository _repository;

    public CategoriesController(ICategorieRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategorieDto>>> GetAll()
    {
        var categories = await _repository.GetAllAsync();
        return Ok(categories.Select(c => new CategorieDto(c.CategorieId, c.Libelle, c.Description)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategorieDto>> GetById(int id)
    {
        var categorie = await _repository.GetByIdAsync(id);
        if (categorie is null)
            return NotFound();
        return Ok(new CategorieDto(categorie.CategorieId, categorie.Libelle, categorie.Description));
    }
}
