namespace Application.SupplierManagement.CreateSupplier;

public sealed record CreateSupplierResponse(
    Guid PublicId,
    string Name,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone);