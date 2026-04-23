using GreenMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenMarket.API.Data.Configurations;

public class LigneCommandeConfiguration : IEntityTypeConfiguration<LigneCommande>
{
    public void Configure(EntityTypeBuilder<LigneCommande> builder)
    {
        builder.ToTable("LignesCommande");

        builder.HasKey(l => new { l.CommandeId, l.ProduitId });

        builder.Property(l => l.PrixUnitaire)
               .HasColumnType("numeric(10,2)");

        builder.Ignore(l => l.SousTotal);

        builder.HasOne(l => l.Produit)
               .WithMany()
               .HasForeignKey(l => l.ProduitId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Producteur)
               .WithMany()
               .HasForeignKey(l => l.ProducteurId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}