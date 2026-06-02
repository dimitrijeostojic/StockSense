namespace Application.SupplierManagement.UpdateSupplier;

public sealed record UpdateSupplierResponse(
    Guid PublicId,
    string Name,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone);