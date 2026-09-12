using Domain.Core;
using MediatR;

namespace Application.TenantManagement.GetMyTenant;

public sealed record GetMyTenantRequest : IRequest<TResult<GetMyTenantResponse>>;
