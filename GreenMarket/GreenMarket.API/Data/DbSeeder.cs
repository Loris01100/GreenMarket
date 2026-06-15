using GreenMarket.Application.Data;
using GreenMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreenMarket.API.Data;

/// <summary>
/// Jeu de données de test : un producteur et quelques produits avec stock,
/// pour pouvoir tester le tunnel de commande + paiement Stripe de bout en bout.
/// Idempotent : ne fait rien si des produits existent déjà.
/// </summary>
public static class DbSeeder
{
    // Utilisateur fictif rattaché au producteur de démonstration.
    private static readonly Guid ProducteurUtilisateurId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static async Task SeedTestDataAsync(GreenMarketDbContext db, ILogger logger)
    {
        if (!await db.Database.CanConnectAsync())
        {
            logger.LogWarning(
                "Seed ignoré : impossible de se connecter à la base. " +
                "Avez-vous appliqué les migrations (dotnet ef database update) ?");
            return;
        }

        if (await db.Produits.AnyAsync())
        {
            logger.LogInformation("Seed ignoré : des produits existent déjà.");
            return;
        }

        logger.LogInformation("Seed des données de test (producteur + produits)...");

        if (!await db.Utilisateurs.AnyAsync(u => u.KeycloakId == ProducteurUtilisateurId))
        {
            db.Utilisateurs.Add(new Utilisateur { KeycloakId = ProducteurUtilisateurId });
        }

        var producteur = new Producteur
        {
            UtilisateurId = ProducteurUtilisateurId,
            NomProducteur = "Ferme des Tilleuls",
            Adresse       = "12 chemin des Prés, 38000 Grenoble",
            Description    = "Producteur local de démonstration (données de test)."
        };
        db.Producteurs.Add(producteur);
        await db.SaveChangesAsync();

        // CategorieId : 1=Légumes, 2=Fruits, 3=Produits laitiers, 4=Produits fermiers
        // (catégories seedées par la migration InitialCreate).
        var produits = new List<Produit>
        {
            Produit(producteur.ProducteurId, 1, "Tomates anciennes bio", "Variétés colorées cueillies à maturité.", 3.50m, 92, 50),
            Produit(producteur.ProducteurId, 1, "Carottes des sables",    "Carottes douces cultivées en plein champ.", 2.20m, 88, 80),
            Produit(producteur.ProducteurId, 2, "Pommes Gala",            "Pommes croquantes du verger.",              2.90m, 80, 60),
            Produit(producteur.ProducteurId, 2, "Fraises de pleine terre","Fraises parfumées de saison.",              4.50m, 75, 30),
            Produit(producteur.ProducteurId, 3, "Fromage de chèvre fermier","Affiné à la ferme, au lait cru.",         5.80m, 70, 25),
            Produit(producteur.ProducteurId, 3, "Yaourt nature au lait cru","Yaourt artisanal en pot de verre.",       1.20m, 85, 100),
            Produit(producteur.ProducteurId, 4, "Miel de printemps",      "Récolte locale, non chauffée.",             7.50m, 95, 40),
            Produit(producteur.ProducteurId, 4, "Œufs plein air (x6)",    "Poules élevées en plein air.",              3.20m, 90, 70),
        };

        db.Produits.AddRange(produits);
        await db.SaveChangesAsync();

        logger.LogInformation("Seed terminé : {Count} produits créés.", produits.Count);
    }

    private static Produit Produit(
        int producteurId, int categorieId, string nom, string description,
        decimal prix, int score, int stock) => new()
    {
        ProducteurId         = producteurId,
        CategorieId          = categorieId,
        Nom                  = nom,
        Description          = description,
        PrixUnitaire         = prix,
        ScoreEnvironnemental = score,
        EstActif             = true,
        Stock                = new Stock { QuantiteDisponible = stock, SeuilAlerte = 5 }
    };
}
