using GreenMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreenMarket.Application.Data;

public class GreenMarketDbContext(DbContextOptions<GreenMarketDbContext> options) : DbContext(options)
{
    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();
    public DbSet<Producteur> Producteurs => Set<Producteur>();
    public DbSet<Categorie> Categories => Set<Categorie>();
    public DbSet<Produit> Produits => Set<Produit>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<Commande> Commandes => Set<Commande>();
    public DbSet<LigneCommande> LignesCommande => Set<LigneCommande>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("greenmarket");

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.ToTable("utilisateur");
            entity.HasKey(u => u.KeycloakId);
            entity.Property(u => u.KeycloakId).HasColumnName("keycloak_id");
            entity.Property(u => u.DateInscription).HasColumnName("date_inscription");
        });

        modelBuilder.Entity<Producteur>(entity =>
        {
            entity.ToTable("producteur");
            entity.HasKey(p => p.ProducteurId);
            entity.Property(p => p.ProducteurId).HasColumnName("producteur_id");
            entity.Property(p => p.UtilisateurId).HasColumnName("utilisateur_id");
            entity.Property(p => p.NomProducteur).HasColumnName("nom_producteur").HasMaxLength(200);
            entity.Property(p => p.Adresse).HasColumnName("adresse").HasMaxLength(500);
            entity.Property(p => p.Description).HasColumnName("description");
            entity.Property(p => p.DateAdhesion).HasColumnName("date_adhesion");

            entity.HasIndex(p => p.UtilisateurId).IsUnique();

            entity.HasOne(p => p.Utilisateur)
                  .WithOne(u => u.Producteur)
                  .HasForeignKey<Producteur>(p => p.UtilisateurId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Categorie>(entity =>
        {
            entity.ToTable("categorie");
            entity.HasKey(c => c.CategorieId);
            entity.Property(c => c.CategorieId).HasColumnName("categorie_id");
            entity.Property(c => c.Libelle).HasColumnName("libelle").HasMaxLength(100);
            entity.Property(c => c.Description).HasColumnName("description");

            entity.HasIndex(c => c.Libelle).IsUnique();
        });

        modelBuilder.Entity<Produit>(entity =>
        {
            entity.ToTable("produit");
            entity.HasKey(p => p.ProduitId);
            entity.Property(p => p.ProduitId).HasColumnName("produit_id");
            entity.Property(p => p.ProducteurId).HasColumnName("producteur_id");
            entity.Property(p => p.CategorieId).HasColumnName("categorie_id");
            entity.Property(p => p.Nom).HasColumnName("nom").HasMaxLength(200);
            entity.Property(p => p.Description).HasColumnName("description");
            entity.Property(p => p.PrixUnitaire).HasColumnName("prix_unitaire").HasColumnType("numeric(10,2)");
            entity.Property(p => p.ScoreEnvironnemental).HasColumnName("score_environnemental");
            entity.Property(p => p.Tracabilite).HasColumnName("tracabilite");
            entity.Property(p => p.EstActif).HasColumnName("est_actif");
            entity.Property(p => p.DateCreation).HasColumnName("date_creation");

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_produit_prix_unitaire", "prix_unitaire >= 0");
                t.HasCheckConstraint("CK_produit_score_environnemental", "score_environnemental BETWEEN 0 AND 100");
            });

            entity.HasOne(p => p.Producteur)
                  .WithMany(pr => pr.Produits)
                  .HasForeignKey(p => p.ProducteurId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Categorie)
                  .WithMany(c => c.Produits)
                  .HasForeignKey(p => p.CategorieId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.ToTable("stock");
            entity.HasKey(s => s.StockId);
            entity.Property(s => s.StockId).HasColumnName("stock_id");
            entity.Property(s => s.ProduitId).HasColumnName("produit_id");
            entity.Property(s => s.QuantiteDisponible).HasColumnName("quantite_disponible");
            entity.Property(s => s.SeuilAlerte).HasColumnName("seuil_alerte");

            entity.HasIndex(s => s.ProduitId).IsUnique();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_stock_quantite_disponible", "quantite_disponible >= 0");
                t.HasCheckConstraint("CK_stock_seuil_alerte", "seuil_alerte >= 0");
            });

            entity.HasOne(s => s.Produit)
                  .WithOne(p => p.Stock)
                  .HasForeignKey<Stock>(s => s.ProduitId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Commande>(entity =>
        {
            entity.ToTable("commande");
            entity.HasKey(c => c.CommandeId);
            entity.Property(c => c.CommandeId).HasColumnName("commande_id");
            entity.Property(c => c.UtilisateurId).HasColumnName("utilisateur_id");
            entity.Property(c => c.DateCommande).HasColumnName("date_commande");
            entity.Property(c => c.MontantTotal).HasColumnName("montant_total").HasColumnType("numeric(10,2)");
            entity.Property(c => c.StatutPaiement).HasColumnName("statut_paiement").HasMaxLength(50);

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_commande_montant_total", "montant_total >= 0");
                t.HasCheckConstraint("CK_commande_statut_paiement",
                    "statut_paiement IN ('en_attente', 'paye', 'refuse', 'rembourse', 'annule')");
            });

            entity.HasOne(c => c.Utilisateur)
                  .WithMany(u => u.Commandes)
                  .HasForeignKey(c => c.UtilisateurId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LigneCommande>(entity =>
        {
            entity.ToTable("ligne_commande");
            entity.HasKey(lc => new { lc.CommandeId, lc.ProduitId });
            entity.Property(lc => lc.CommandeId).HasColumnName("commande_id");
            entity.Property(lc => lc.ProducteurId).HasColumnName("producteur_id");
            entity.Property(lc => lc.ProduitId).HasColumnName("produit_id");
            entity.Property(lc => lc.Quantite).HasColumnName("quantite");
            entity.Property(lc => lc.PrixUnitaire).HasColumnName("prix_unitaire").HasColumnType("numeric(10,2)");

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ligne_commande_quantite", "quantite > 0");
                t.HasCheckConstraint("CK_ligne_commande_prix_unitaire", "prix_unitaire >= 0");
            });

            entity.HasOne(lc => lc.Commande)
                  .WithMany(c => c.LignesCommande)
                  .HasForeignKey(lc => lc.CommandeId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(lc => lc.Producteur)
                  .WithMany(p => p.LignesCommande)
                  .HasForeignKey(lc => lc.ProducteurId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(lc => lc.Produit)
                  .WithMany(p => p.LignesCommande)
                  .HasForeignKey(lc => lc.ProduitId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
