using Application.Abstractions.Services;
using Application.Common.Errors;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.TenantManagement.GetMyTenant;

internal sealed class GetMyTenantRequestHandler(
    ITenantRepository tenantRepository,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<GetMyTenantRequest, TResult<GetMyTenantResponse>>
{
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;

    public async Task<TResult<GetMyTenantResponse>> Handle(GetMyTenantRequest request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByPublicIdAsync(_currentUserAccessor.TenantPublicId, cancellationToken);
        if (tenant == null)
        {
            return TResult<GetMyTenantResponse>.Failure(ApplicationErrors.NotFound);
        }
        var response = new GetMyTenantResponse(
            tenant.PublicId,
            tenant.Name,
            tenant.Pib,
            tenant.Address,
            tenant.LogoUrl);
        return TResult<GetMyTenantResponse>.Success(response);
    }
}
