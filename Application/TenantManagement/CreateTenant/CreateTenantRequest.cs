using Domain.Core;
using MediatR;

namespace Application.TenantManagement.CreateTenant;

public sealed record CreateTenantRequest(
    string TenantName,
    string Pib,
    string Address,
    string AdminEmail,
    string AdminFirstName,
    string AdminLastName) : IRequest<TResult<CreateTenantResponse>>;
