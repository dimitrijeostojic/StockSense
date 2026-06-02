namespace Application.SupplierManagement.GetAllSuppliers;

public sealed record GetAllSuppliersDto(
    Guid PublicId,
    string Name,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone);