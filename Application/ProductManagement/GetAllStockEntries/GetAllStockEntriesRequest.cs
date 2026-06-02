using Domain.Core;
using MediatR;

namespace Application.ProductManagement.GetAllStockEntries;

public sealed record GetAllStockEntriesRequest(Guid ProductPublicId) : IRequest<TResult<GetAllStockEntriesResponse>>;