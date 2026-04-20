using System.ComponentModel.DataAnnotations;

namespace GreenMarket.Domain.Entities;

public class Utilisateur
{
    [Key]
    public Guid KeycloakId { get; set; }

    public DateTimeOffset DateInscription { get; set; } = DateTimeOffset.UtcNow;

    public virtual Producteur? Producteur { get; set; }
    public virtual ICollection<Commande> Commandes { get; set; } = new List<Commande>();
}
