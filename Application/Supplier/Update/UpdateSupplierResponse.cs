namespace Application.Supplier.Update;

public sealed record UpdateSupplierResponse(
    Guid PublicId,
    string Name,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone);