namespace Application.ProductManagement.BulkImport;

public sealed record ImportRowError(int RowNumber, string ErrorMessage);
