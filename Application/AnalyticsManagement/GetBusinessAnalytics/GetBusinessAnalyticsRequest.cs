using Application.AnalyticsManagement.Common;
using Domain.Core;
using MediatR;

namespace Application.AnalyticsManagement.GetBusinessAnalytics;

public record GetBusinessAnalyticsRequest(
    TimeRangeQuery TimeRange,
    int TopN = 5,
    int StockPage = 1,
    int StockPageSize = 5)
    : IRequest<TResult<GetBusinessAnalyticsResponse>>;
