using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace UnitTests.Domain;

public sealed class ApplicationUserEntityTests
{
    [Fact]
    public void Create_HasSeenOnboardingDefaultsToFalse()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);

        user.HasSeenOnboarding.Should().BeFalse();
    }

    [Fact]
    public void CompleteOnboarding_SetsHasSeenOnboardingToTrue()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);

        user.CompleteOnboarding();

        user.HasSeenOnboarding.Should().BeTrue();
    }

    [Fact]
    public void CompleteOnboarding_CalledTwice_RemainsTrue()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);

        user.CompleteOnboarding();
        user.CompleteOnboarding();

        user.HasSeenOnboarding.Should().BeTrue();
    }
}
