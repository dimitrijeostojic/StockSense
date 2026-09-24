namespace Application.AnalyticsManagement.GetUserAnalytics;

public record GetUserAnalyticsResponse(
    IReadOnlyList<ActivityByEntityDto> ActivityByEntityType,
    IReadOnlyList<ActivityByActionDto> ActivityByActionType,
    IReadOnlyList<TopActiveUserDto> TopActiveUsers,
    IReadOnlyList<RegistrationTrendPointDto> RegistrationTrend);

public record ActivityByEntityDto(string EntityName, int Count);
public record ActivityByActionDto(string Action, int Count);
public record TopActiveUserDto(string UserEmail, int Count);
public record RegistrationTrendPointDto(string Period, int Count);
