namespace Application.SupplierManagement.GetSupplierById;

public sealed record GetSupplierByIdResponse(
   Guid PublicId,
    string Name,
    string SupplierCode,
    string ContactName,
    string ContactEmail,
    string? ContactPhone,
    string? Address,
    string? City,
    string? Country);
