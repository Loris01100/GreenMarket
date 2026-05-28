using GreenMarket.Domain.Entities;
using GreenMarket.Domain.Interfaces;
using GreenMarket.Shared.DTOs.Produits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenMarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProduitsController : ControllerBase
{
    private readonly IProduitRepository _repository;

    public ProduitsController(IProduitRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProduitDto>>> GetAll()
    {
        var produits = await _repository.GetAllAsync();
        return Ok(produits.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProduitDto>> GetById(int id)
    {
        var produit = await _repository.GetByIdAsync(id);
        if (produit is null)
            return NotFound();
        return Ok(ToDto(produit));
    }

    [HttpPost]
    [Authorize(Roles = "Producteur,Admin")]
    public async Task<ActionResult<ProduitDto>> Create(ProduitCreateDto dto)
    {
        var produit = new Produit
        {
            Nom = dto.Nom,
            Description = dto.Description,
            PrixUnitaire = dto.PrixUnitaire,
            ProducteurId = dto.ProducteurId,
            CategorieId = dto.CategorieId,
            EstActif = true,
            DateCreation = DateTimeOffset.UtcNow
        };

        await _repository.AddAsync(produit);
        var created = await _repository.GetByIdAsync(produit.ProduitId);
        return CreatedAtAction(nameof(GetById), new { id = produit.ProduitId }, ToDto(created!));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Producteur,Admin")]
    public async Task<IActionResult> Update(int id, ProduitCreateDto dto)
    {
        var produit = await _repository.GetByIdAsync(id);
        if (produit is null)
            return NotFound();

        produit.Nom = dto.Nom;
        produit.Description = dto.Description;
        produit.PrixUnitaire = dto.PrixUnitaire;
        produit.CategorieId = dto.CategorieId;

        await _repository.UpdateAsync(produit);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var produit = await _repository.GetByIdAsync(id);
        if (produit is null)
            return NotFound();

        await _repository.DeleteAsync(id);
        return NoContent();
    }

    private static ProduitDto ToDto(Produit p) => new(
        p.ProduitId,
        p.Nom,
        p.Description,
        p.PrixUnitaire,
        p.EstActif,
        p.ProducteurId,
        p.Producteur?.NomProducteur ?? string.Empty,
        p.CategorieId,
        p.Categorie?.Libelle ?? string.Empty,
        p.Stock?.QuantiteDisponible
    );
}
