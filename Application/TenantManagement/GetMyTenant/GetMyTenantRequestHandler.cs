using Application.Abstractions.Services;
using Application.Common.Errors;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.TenantManagement.GetMyTenant;

internal sealed class GetMyTenantRequestHandler(
    ITenantRepository tenantRepository,
    ICurrentUserAccessor currentUserAccessor,
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<GetMyTenantRequest, TResult<GetMyTenantResponse>>
{
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<TResult<GetMyTenantResponse>> Handle(GetMyTenantRequest request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByPublicIdAsync(_currentUserAccessor.TenantPublicId, cancellationToken);
        if (tenant is null)
        {
            return TResult<GetMyTenantResponse>.Failure(ApplicationErrors.NotFound);
        }

        var user = await _userManager.FindByIdAsync(_currentUserAccessor.UserId);
        var seenTourPages = user?.GetSeenTourPages() ?? [];

        return TResult<GetMyTenantResponse>.Success(new GetMyTenantResponse(
            tenant.PublicId,
            tenant.Name,
            tenant.Pib,
            tenant.Address,
            tenant.Logo,
            seenTourPages));
    }
}
