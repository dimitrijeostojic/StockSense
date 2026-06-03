using Domain.Core;
using MediatR;

namespace Application.AuthManagement.Register;

public sealed record RegisterRequest(
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Password,
    string CompanyName,
    string PIB,
    string Address) : IRequest<TResult<RegisterResponse>>;
