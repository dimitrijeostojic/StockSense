namespace Application.TenantManagement.Update;

public sealed record UpdateTenantResponse(Guid PublicId, string Name, string PIB, string? Address, byte[]? Logo);
