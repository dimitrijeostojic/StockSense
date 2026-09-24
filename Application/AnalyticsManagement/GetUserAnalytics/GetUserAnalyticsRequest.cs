using Application.AnalyticsManagement.Common;
using Domain.Core;
using MediatR;

namespace Application.AnalyticsManagement.GetUserAnalytics;

public record GetUserAnalyticsRequest(TimeRangeQuery TimeRange, int TopN = 10)
    : IRequest<TResult<GetUserAnalyticsResponse>>;
