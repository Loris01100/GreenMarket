namespace GreenMarket.Application.Interfaces;

public interface IKeycloakService
{
    Task AssignerRoleAsync(Guid keycloakUserId, string role);
    Task SupprimerRoleAsync(Guid keycloakUserId, string role);
}
