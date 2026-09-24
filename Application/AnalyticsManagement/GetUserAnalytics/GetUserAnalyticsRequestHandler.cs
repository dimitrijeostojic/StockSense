using Application.Abstractions.Services;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.AnalyticsManagement.GetUserAnalytics;

internal sealed class GetUserAnalyticsRequestHandler(
    IAnalyticsRepository analyticsRepository,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<GetUserAnalyticsRequest, TResult<GetUserAnalyticsResponse>>
{
    private readonly IAnalyticsRepository _analyticsRepository = analyticsRepository ?? throw new ArgumentNullException(nameof(analyticsRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<GetUserAnalyticsResponse>> Handle(GetUserAnalyticsRequest request, CancellationToken cancellationToken)
    {
        var tenantPublicId = _currentUserAccessor.TenantPublicId;
        var from = request.TimeRange.From;
        var to = request.TimeRange.To;

        var userIds = await _analyticsRepository.GetUserIdsByTenantAsync(tenantPublicId, cancellationToken);

        var activityByEntity = await _analyticsRepository.GetActivityByEntityTypeAsync(userIds, from, to, cancellationToken);
        var activityByAction = await _analyticsRepository.GetActivityByActionTypeAsync(userIds, from, to, cancellationToken);
        var topUsers = await _analyticsRepository.GetTopActiveUsersAsync(userIds, from, to, request.TopN, cancellationToken);
        var registrationTrend = await _analyticsRepository.GetRegistrationTrendAsync(tenantPublicId, from, to, cancellationToken);

        var response = new GetUserAnalyticsResponse(
            activityByEntity.Select(x => new ActivityByEntityDto(x.EntityName, x.Count)).ToList(),
            activityByAction.Select(x => new ActivityByActionDto(x.Action, x.Count)).ToList(),
            topUsers.Select(x => new TopActiveUserDto(x.UserEmail, x.Count)).ToList(),
            registrationTrend.Select(x => new RegistrationTrendPointDto(x.Period, x.Count)).ToList());

        return TResult<GetUserAnalyticsResponse>.Success(response);
    }
}
