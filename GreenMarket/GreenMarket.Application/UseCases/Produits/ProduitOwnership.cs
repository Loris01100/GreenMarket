using GreenMarket.Domain.Interfaces;

namespace GreenMarket.Application.UseCases.Produits;

/// <summary>
/// Règle transverse F5.4 / F6 : un producteur ne peut gérer que les produits qu'il
/// a référencés. L'administrateur (supervision, F8) n'est pas restreint.
/// </summary>
internal static class ProduitOwnership
{
    public static async Task EnsureCanManage(
        int produitProducteurId,
        Guid utilisateurId,
        bool estAdmin,
        IProducteurRepository producteurRepository)
    {
        if (estAdmin)
            return;

        var producteur = await producteurRepository.GetByUtilisateurIdAsync(utilisateurId)
            ?? throw new UnauthorizedAccessException(
                "Aucun profil producteur n'est associé à ce compte.");

        if (producteur.ProducteurId != produitProducteurId)
            throw new UnauthorizedAccessException(
                "Accès refusé : ce produit appartient à un autre producteur.");
    }
}
