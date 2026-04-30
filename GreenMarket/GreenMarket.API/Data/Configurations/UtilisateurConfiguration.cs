using GreenMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenMarket.API.Data.Configurations;

public class UtilisateurConfiguration : IEntityTypeConfiguration<Utilisateur>
{
    public void Configure(EntityTypeBuilder<Utilisateur> builder)
    {
        builder.ToTable("utilisateur", "greenmarket");
        builder.HasKey(u => u.KeycloakId);
        builder.Property(u => u.KeycloakId).HasColumnName("keycloak_id");
        builder.Property(u => u.DateInscription).HasColumnName("date_inscription");
    }
}
