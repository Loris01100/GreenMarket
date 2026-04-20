using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenMarket.Domain.Entities;

public class Commande
{
    [Key]
    public int CommandeId { get; set; }

    public Guid UtilisateurId { get; set; }

    [ForeignKey(nameof(UtilisateurId))]
    public virtual Utilisateur Utilisateur { get; set; } = null!;

    public DateTimeOffset DateCommande { get; set; } = DateTimeOffset.UtcNow;

    [Column(TypeName = "numeric(10,2)")]
    public decimal MontantTotal { get; set; }

    [Required]
    [MaxLength(50)]
    public string StatutPaiement { get; set; } = "en_attente";

    public virtual ICollection<LigneCommande> LignesCommande { get; set; } = new List<LigneCommande>();
}
