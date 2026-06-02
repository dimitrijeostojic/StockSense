namespace Application.OrderManagement.UpdateOrder;

public sealed record UpdateOrderRequestBody(Guid SupplierPublicId,
    DateTime OrderDate,
    string? Notes);
