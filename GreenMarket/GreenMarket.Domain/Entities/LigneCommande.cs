using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenMarket.Domain.Entities;

public class LigneCommande
{
    public int CommandeId { get; set; }
    [ForeignKey(nameof(CommandeId))]
    public virtual Commande Commande { get; set; } = null!;

    public int ProduitId { get; set; }
    [ForeignKey(nameof(ProduitId))]
    public virtual Produit Produit { get; set; } = null!;

    public int ProducteurId { get; set; }
    [ForeignKey(nameof(ProducteurId))]
    public virtual Producteur Producteur { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int Quantite { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal PrixUnitaire { get; set; }

    [NotMapped]
    public decimal SousTotal => PrixUnitaire * Quantite;
}