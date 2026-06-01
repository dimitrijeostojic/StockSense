namespace Application.Supplier.GetAll;

public sealed record GetAllSuppliersDto(
    Guid PublicId,
    string Name,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone);