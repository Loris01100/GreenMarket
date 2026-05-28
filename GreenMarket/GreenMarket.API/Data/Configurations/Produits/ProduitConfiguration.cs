using GreenMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenMarket.API.Data.Configurations.Produits;

public class ProduitConfiguration : IEntityTypeConfiguration<Produit>
{
    public void Configure(EntityTypeBuilder<Produit> builder)
    {
        builder.HasKey(p => p.ProduitId);

        builder.Property(p => p.Nom)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.PrixUnitaire)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Stock)
            .IsRequired();

        builder.HasOne(p => p.Categorie)
            .WithMany(c => c.Produits)
            .HasForeignKey(p => p.CategorieId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
