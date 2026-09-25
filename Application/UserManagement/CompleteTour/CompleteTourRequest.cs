using Domain.Core;
using MediatR;

namespace Application.UserManagement.CompleteTour;

public sealed record CompleteTourRequest(string PageName) : IRequest<Result>;
