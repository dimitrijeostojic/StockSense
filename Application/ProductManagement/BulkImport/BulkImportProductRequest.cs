using Application.Abstractions.Idempotency;
using Domain.Core;

namespace Application.ProductManagement.BulkImport;

public sealed record BulkImportProductRequest(byte[] FileContent, Guid RequestId) : IdempotentRequest<TResult<BulkImportProductResponse>>(RequestId);
