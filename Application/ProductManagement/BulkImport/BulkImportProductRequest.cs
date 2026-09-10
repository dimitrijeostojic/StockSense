using Domain.Core;
using MediatR;

namespace Application.ProductManagement.BulkImport;

public sealed record BulkImportProductRequest(byte[] FileContent) : IRequest<TResult<BulkImportProductResponse>>;
