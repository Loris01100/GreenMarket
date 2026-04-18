using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenMarket.Domain.Entities;

public class Stock
{
    [Key]
    public int StockId { get; set; }

    public int ProduitId { get; set; }

    [ForeignKey(nameof(ProduitId))]
    public virtual Produit Produit { get; set; } = null!;

    public int QuantiteDisponible { get; set; } = 0;

    public int SeuilAlerte { get; set; } = 5;
}
