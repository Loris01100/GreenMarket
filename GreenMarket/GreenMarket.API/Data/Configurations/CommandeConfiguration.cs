using GreenMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenMarket.API.Data.Configurations;

public class CommandeConfiguration : IEntityTypeConfiguration<Commande>
{
    public void Configure(EntityTypeBuilder<Commande> builder)
    {
        builder.ToTable("Commandes");

        builder.HasKey(c => c.CommandeId);

        builder.Property(c => c.StatutPaiement)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(c => c.MontantTotal)
               .HasColumnType("numeric(10,2)");

        builder.HasOne(c => c.Utilisateur)
               .WithMany()
               .HasForeignKey(c => c.UtilisateurId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.LignesCommande)
               .WithOne(l => l.Commande)
               .HasForeignKey(l => l.CommandeId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}