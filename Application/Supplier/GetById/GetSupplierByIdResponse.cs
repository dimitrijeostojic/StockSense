namespace Application.Supplier.GetById;

public sealed record GetSupplierByIdResponse(
    Guid PublicId,
    string Name,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone);
