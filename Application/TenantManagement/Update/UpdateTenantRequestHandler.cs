using Application.Abstractions.Services;
using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.TenantManagement.Update;

internal sealed class UpdateTenantRequestHandler(
    ITenantRepository tenantRepository,
    IAuthUnitOfWork authUnitOfWork,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<UpdateTenantRequest, TResult<UpdateTenantResponse>>
{
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
    private readonly IAuthUnitOfWork _authUnitOfWork = authUnitOfWork ?? throw new ArgumentNullException(nameof(authUnitOfWork));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<UpdateTenantResponse>> Handle(UpdateTenantRequest request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByPublicIdAsync(_currentUserAccessor.TenantPublicId, cancellationToken);
        if (tenant == null)
        {
            return TResult<UpdateTenantResponse>.Failure(ApplicationErrors.NotFound);
        }
        tenant.WithName(request.Name)
            .WithAddress(request.Address)
            .WithLogoUrl(request.LogoUrl);

        await _authUnitOfWork.SaveChangesAsync(cancellationToken);

        return TResult<UpdateTenantResponse>.Success(new UpdateTenantResponse(tenant.PublicId, tenant.Name, tenant.Pib, tenant.Address, tenant.LogoUrl));
    }
}
