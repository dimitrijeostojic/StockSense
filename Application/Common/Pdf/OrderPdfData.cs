namespace Application.Common.Pdf;

public sealed record OrderPdfData(
    string OrderPublicId,
    DateTime OrderDate,
    string TenantName,
    string TenantPib,
    string TenantAddress,
    string SupplierName,
    string? SupplierContactEmail,
    string? SupplierContactPhone,
    IReadOnlyCollection<OrderPdfItem> Items);