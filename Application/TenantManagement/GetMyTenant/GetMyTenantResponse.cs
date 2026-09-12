namespace Application.TenantManagement.GetMyTenant;

public sealed record GetMyTenantResponse(Guid PublicId, string Name, string PIB, string? Address, string? LogoUrl);
