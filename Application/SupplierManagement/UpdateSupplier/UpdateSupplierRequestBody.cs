namespace Application.SupplierManagement.UpdateSupplier;

public sealed record UpdateSupplierRequestBody(
    string Name,
    string ContactName,
    string ContactEmail,
    string SupplierCode,
    string? ContactPhone,
    string? Address,
    string? City,
    string? Country);
