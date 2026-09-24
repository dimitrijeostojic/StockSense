using Application.Abstractions.Services;
using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.TenantManagement.CompleteOnboarding;

internal sealed class CompleteOnboardingRequestHandler(
    UserManager<ApplicationUser> userManager,
    IAuthUnitOfWork authUnitOfWork,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<CompleteOnboardingRequest, TResult<CompleteOnboardingResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IAuthUnitOfWork _authUnitOfWork = authUnitOfWork ?? throw new ArgumentNullException(nameof(authUnitOfWork));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<CompleteOnboardingResponse>> Handle(CompleteOnboardingRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(_currentUserAccessor.UserId);
        if (user is null)
        {
            return TResult<CompleteOnboardingResponse>.Failure(ApplicationErrors.NotFound);
        }

        user.CompleteOnboarding();
        await _authUnitOfWork.SaveChangesAsync(cancellationToken);

        return TResult<CompleteOnboardingResponse>.Success(new CompleteOnboardingResponse());
    }
}
