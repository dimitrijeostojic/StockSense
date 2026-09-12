using Domain.Core;
using MediatR;

namespace Application.TenantManagement.Update;

public sealed record UpdateTenantRequest(string Name, string? Address, string? LogoUrl) : IRequest<TResult<UpdateTenantResponse>>;
