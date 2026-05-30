namespace GreenMarket.Domain.Exceptions;

public class ProduitInactifException : Exception
{
    public int ProduitId { get; }

    public ProduitInactifException(int produitId)
        : base($"Le produit {produitId} est inactif et ne peut pas être commandé.")
    {
        ProduitId = produitId;
    }
}