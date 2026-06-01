namespace Application.Supplier.Update;

public sealed record UpdateSupplierRequestBody(
    string Name,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone);