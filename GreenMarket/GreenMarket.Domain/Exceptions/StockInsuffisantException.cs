namespace GreenMarket.Domain.Exceptions;

public class StockInsuffisantException : Exception
{
    public int ProduitId { get; }
    public int QuantiteDemandee { get; }
    public int QuantiteDisponible { get; }

    public StockInsuffisantException(int produitId, int quantiteDemandee, int quantiteDisponible)
        : base($"Stock insuffisant pour le produit {produitId} : {quantiteDemandee} demandé(s), {quantiteDisponible} disponible(s).")
    {
        ProduitId = produitId;
        QuantiteDemandee = quantiteDemandee;
        QuantiteDisponible = quantiteDisponible;
    }
}