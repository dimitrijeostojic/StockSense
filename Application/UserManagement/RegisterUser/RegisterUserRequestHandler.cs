using Application.Abstractions.Services;
using Application.Common.Constants;
using Application.Common.Errors;
using Application.Emails;
using Domain.Abstractions;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.UserManagement.RegisterUser;

internal sealed class RegisterUserRequestHandler(
    ICurrentUserAccessor currentUserAccessor,
    ITenantRepository tenantRepository,
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IUnitOfWork unitOfWork,
    ILogger<RegisterUserRequestHandler> logger)
    : IRequestHandler<RegisterUserRequest, TResult<RegisterUserResponse>>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ILogger<RegisterUserRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<TResult<RegisterUserResponse>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByPublicIdAsync(_currentUserAccessor.TenantPublicId, cancellationToken);
        if (tenant == null)
        {
            return TResult<RegisterUserResponse>.Failure(ApplicationErrors.NotFound);
        }
        var existingUser = tenant.ApplicationUsers.FirstOrDefault(u => u.Email == request.Email);
        if (existingUser != null)
        {
            return TResult<RegisterUserResponse>.Failure(ApplicationErrors.EmailAlreadyExists);
        }
        var user = ApplicationUser.Create(request.Username, request.Email, request.FirstName, request.LastName, tenant.Id);
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return TResult<RegisterUserResponse>.Failure(new Error("Register.RegisterUser", string.Join(", ", result.Errors.Select(e => e.Description))));
        }
        var roleResult = await _userManager.AddToRoleAsync(user, Roles.User);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return TResult<RegisterUserResponse>.Failure(new Error("Register.RegisterUser", string.Join(", ", roleResult.Errors.Select(e => e.Description))));
        }

        try
        {
            var message = EmailTemplates.UserInvited(request.Email, request.FirstName, tenant.Name);
            await _emailService.SendAsync(message, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to queue welcome email for {Email}", request.Email);
        }
        return TResult<RegisterUserResponse>.Success(new RegisterUserResponse("User successfully registered"));
    }
}
