namespace Application.TenantManagement.CreateTenant;

public sealed record CreateTenantResponse(Guid TenantPublicId, string TenantName, string AdminEmail);
