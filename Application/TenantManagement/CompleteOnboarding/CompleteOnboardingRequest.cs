using Domain.Core;
using MediatR;

namespace Application.TenantManagement.CompleteOnboarding;

public sealed record CompleteOnboardingRequest : IRequest<TResult<CompleteOnboardingResponse>>;
