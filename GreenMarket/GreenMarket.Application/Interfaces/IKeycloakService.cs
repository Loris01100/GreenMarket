namespace GreenMarket.Application.Interfaces;

public interface IKeycloakService
{
    Task<bool> UserExistsAsync(Guid keycloakId);
    Task AssignRoleAsync(Guid keycloakId, string role);
    Task RemoveRoleAsync(Guid keycloakId, string role);
}
