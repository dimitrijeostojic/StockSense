namespace Application.SupplierManagement.GetSupplierById;

public sealed record GetSupplierByIdResponse(
    Guid PublicId,
    string Name,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone);
