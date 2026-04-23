using System.Security.Claims;
using GreenMarket.Application.Mappings;
using GreenMarket.Application.UseCases.Producteurs;
using GreenMarket.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenMarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProducteursController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProducteursController(IMediator mediator)
        => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProducteurDto>>> GetAll()
    {
        var producteurs = await _mediator.Send(new GetProducteursQuery());
        return Ok(producteurs.Select(ProducteurMappingExtensions.ToDto));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProducteurDto>> GetById(int id)
    {
        var producteur = await _mediator.Send(new GetProducteurByIdQuery(id));
        return producteur is null ? NotFound() : Ok(ProducteurMappingExtensions.ToDto(producteur));
    }

    [HttpPost]
    public async Task<ActionResult<ProducteurDto>> Create([FromBody] ProducteurCreateDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var producteur = await _mediator.Send(new CreateProducteurCommand(userId, dto));
        return CreatedAtAction(nameof(GetById), new { id = producteur.ProducteurId }, producteur);
    }

    [HttpGet("mon-profil")]
    public async Task<ActionResult<ProducteurDto>> GetMonProfil()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var producteur = await _mediator.Send(new GetProducteurByUtilisateurQuery(userId));
        return producteur is null ? NotFound() : Ok(ProducteurMappingExtensions.ToDto(producteur));
    }
}
