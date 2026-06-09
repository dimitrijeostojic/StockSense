using Domain.Core;
using MediatR;

namespace Application.DashboardManagement.Get;

public sealed record GetDashboardRequest() : IRequest<TResult<GetDashboardResponse>>;