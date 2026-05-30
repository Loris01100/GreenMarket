using System.ComponentModel.DataAnnotations;

namespace GreenMarket.Shared.DTOs.Produits;

/// <summary>
/// Données de création / modification d'un produit (F5).
/// Le producteur propriétaire est déduit du jeton d'authentification :
/// il n'est jamais transmis par le client.
/// </summary>
public record ProduitCreateDto(
    [Required(ErrorMessage = "Le nom du produit est obligatoire.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Le nom doit contenir entre 1 et 200 caractères.")]
    string Nom,

    [StringLength(2000, ErrorMessage = "La description ne peut pas dépasser 2000 caractères.")]
    string? Description,

    [Range(0.0, 1_000_000.0, ErrorMessage = "Le prix unitaire doit être positif.")]
    decimal PrixUnitaire,

    [Range(1, int.MaxValue, ErrorMessage = "Une catégorie valide est obligatoire.")]
    int CategorieId,

    [Range(0, 100, ErrorMessage = "Le score environnemental doit être compris entre 0 et 100.")]
    int? ScoreEnvironnemental = null,

    string? Tracabilite = null,

    [Range(0, int.MaxValue, ErrorMessage = "La quantité initiale doit être positive.")]
    int QuantiteInitiale = 0,

    [Range(0, int.MaxValue, ErrorMessage = "Le seuil d'alerte doit être positif.")]
    int SeuilAlerte = 5
);
