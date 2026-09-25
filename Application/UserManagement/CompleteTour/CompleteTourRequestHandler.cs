using Application.Abstractions.Services;
using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UserManagement.CompleteTour;

internal sealed class CompleteTourRequestHandler(
    UserManager<ApplicationUser> userManager,
    IAuthUnitOfWork authUnitOfWork,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<CompleteTourRequest, Result>
{
    private static readonly HashSet<string> AllowedPages = ["dashboard", "products", "suppliers", "orders", "categories", "users"];

    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IAuthUnitOfWork _authUnitOfWork = authUnitOfWork ?? throw new ArgumentNullException(nameof(authUnitOfWork));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<Result> Handle(CompleteTourRequest request, CancellationToken cancellationToken)
    {
        if (!AllowedPages.Contains(request.PageName))
        {
            return Result.Failure(ApplicationErrors.InvalidPageName);
        }

        var user = await _userManager.FindByIdAsync(_currentUserAccessor.UserId);
        if (user is null)
        {
            return Result.Failure(ApplicationErrors.NotFound);
        }

        user.CompleteTour(request.PageName);
        await _authUnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
