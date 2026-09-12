using Domain.Core;
using MediatR;

namespace Application.TenantManagement.Update;

public sealed record UpdateTenantRequest(string Name, string? Address, byte[]? Logo) : IRequest<TResult<UpdateTenantResponse>>;
