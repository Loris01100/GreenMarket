using System.ComponentModel.DataAnnotations;

namespace GreenMarket.Domain.Entities;

public class Categorie
{
    [Key]
    public int CategorieId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Libelle { get; set; } = string.Empty;

    public string? Description { get; set; }

    public virtual ICollection<Produit> Produits { get; set; } = new List<Produit>();
}
