namespace Application.Supplier.Create;

public sealed record CreateSupplierResponse(
    Guid PublicId,
    string Name,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone);