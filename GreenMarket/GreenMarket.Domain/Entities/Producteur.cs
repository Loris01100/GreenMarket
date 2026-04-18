using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenMarket.Domain.Entities;

public class Producteur
{
    [Key]
    public int ProducteurId { get; set; }

    public Guid UtilisateurId { get; set; }

    [ForeignKey(nameof(UtilisateurId))]
    public virtual Utilisateur Utilisateur { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string NomProducteur { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Adresse { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTimeOffset DateAdhesion { get; set; } = DateTimeOffset.UtcNow;

    public virtual ICollection<Produit> Produits { get; set; } = new List<Produit>();
    public virtual ICollection<LigneCommande> LignesCommande { get; set; } = new List<LigneCommande>();
}
