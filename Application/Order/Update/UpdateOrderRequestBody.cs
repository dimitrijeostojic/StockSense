namespace Application.Order.Update;

public sealed record UpdateOrderRequestBody(Guid SupplierPublicId,
    DateTime OrderDate,
    string? Notes);
