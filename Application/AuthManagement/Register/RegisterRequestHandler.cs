using Application.Common.Errors;
using Application.Constants;
using Domain.Abstractions;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Application.AuthManagement.Register;

internal sealed class RegisterRequestHandler
    (UserManager<ApplicationUser> userManager,
    ITenantRepository tenantRepository,
    IAuthUnitOfWork authUnitOfWork)
        : IRequestHandler<RegisterRequest, TResult<RegisterResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
    private readonly IAuthUnitOfWork _authUnitOfWork = authUnitOfWork ?? throw new ArgumentNullException(nameof(authUnitOfWork));

    public async Task<TResult<RegisterResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user != null)
        {
            return TResult<RegisterResponse>.Failure(ApplicationErrors.EmailAlreadyExists);
        }
        var tenant = Tenant.Create(request.CompanyName, request.PIB, request.Address);
        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _authUnitOfWork.SaveChangesAsync(cancellationToken);

        user = ApplicationUser.Create(request.Username, request.Email, request.FirstName, request.LastName, tenant.Id);

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            _tenantRepository.Delete(tenant);
            await _authUnitOfWork.SaveChangesAsync(cancellationToken);
            return TResult<RegisterResponse>.Failure(new Error("Register.Error", string.Join(", ", result.Errors.Select(e => e.Description))));
        }
        var roleResult = await _userManager.AddToRoleAsync(user, Roles.Admin);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            _tenantRepository.Delete(tenant);
            await _authUnitOfWork.SaveChangesAsync(cancellationToken);
            return TResult<RegisterResponse>.Failure(
                new Error("Register.Error", roleResult.Errors.First().Description));
        }

        return TResult<RegisterResponse>.Success(new RegisterResponse("User registered successfully."));
    }
}
