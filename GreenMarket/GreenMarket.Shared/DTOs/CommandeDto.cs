using GreenMarket.Domain.Entities;

namespace GreenMarket.Application.DTOs;

public record CommandeDto(
    int    CommandeId,
    Guid   UtilisateurId,
    DateTimeOffset DateCommande,
    decimal MontantTotal,
    string  StatutPaiement,
    IEnumerable<LigneCommandeDto> Lignes
);

public static class CommandeMappingExtensions
{
    public static CommandeDto ToDto(Commande c) => new(
        c.CommandeId,
        c.UtilisateurId,
        c.DateCommande,
        c.MontantTotal,
        c.StatutPaiement,
        c.LignesCommande.Select(LigneCommandeMappingExtensions.ToDto)
    );
}