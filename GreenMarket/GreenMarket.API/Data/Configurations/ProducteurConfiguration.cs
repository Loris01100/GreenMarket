using GreenMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenMarket.API.Data.Configurations;

public class ProducteurConfiguration : IEntityTypeConfiguration<Producteur>
{
    public void Configure(EntityTypeBuilder<Producteur> builder)
    {
        builder.ToTable("producteur", "greenmarket");
        builder.HasKey(p => p.ProducteurId);
        builder.Property(p => p.ProducteurId).HasColumnName("producteur_id");
        builder.Property(p => p.UtilisateurId).HasColumnName("utilisateur_id");
        builder.Property(p => p.NomProducteur).HasColumnName("nom_producteur").HasMaxLength(200).IsRequired();
        builder.Property(p => p.Adresse).HasColumnName("adresse").HasMaxLength(500).IsRequired();
        builder.Property(p => p.Description).HasColumnName("description");
        builder.Property(p => p.DateAdhesion).HasColumnName("date_adhesion");

        builder.HasIndex(p => p.UtilisateurId).IsUnique();

        builder.HasOne(p => p.Utilisateur)
               .WithOne(u => u.Producteur)
               .HasForeignKey<Producteur>(p => p.UtilisateurId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
