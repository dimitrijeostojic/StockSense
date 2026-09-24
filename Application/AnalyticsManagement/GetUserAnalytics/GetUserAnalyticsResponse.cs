using Application.AnalyticsManagement.Common;

namespace Application.AnalyticsManagement.GetUserAnalytics;

public record GetUserAnalyticsResponse(
    IReadOnlyList<NamedCountDto> ActivityByEntityType,
    IReadOnlyList<NamedCountDto> ActivityByActionType,
    IReadOnlyList<NamedCountDto> TopActiveUsers,
    IReadOnlyList<NamedCountDto> RegistrationTrend);
