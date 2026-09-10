namespace Application.ProductManagement.BulkImport;

public sealed record BulkImportProductResponse(int SuccessCount,
    int FailureCount,
    IReadOnlyCollection<ImportRowError> Errors);
