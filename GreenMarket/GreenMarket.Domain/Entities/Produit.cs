using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenMarket.Domain.Entities;

public class Produit
{
    [Key]
    public int ProduitId { get; set; }

    public int ProducteurId { get; set; }

    [ForeignKey(nameof(ProducteurId))]
    public virtual Producteur Producteur { get; set; } = null!;

    public int CategorieId { get; set; }

    [ForeignKey(nameof(CategorieId))]
    public virtual Categorie Categorie { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Nom { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal PrixUnitaire { get; set; }

    public int? ScoreEnvironnemental { get; set; }

    public string? Tracabilite { get; set; }

    public bool EstActif { get; set; } = true;

    public DateTimeOffset DateCreation { get; set; } = DateTimeOffset.UtcNow;

    public virtual Stock? Stock { get; set; }
    public virtual ICollection<LigneCommande> LignesCommande { get; set; } = new List<LigneCommande>();
}
