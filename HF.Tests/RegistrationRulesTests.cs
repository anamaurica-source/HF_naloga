
using HF.Api.Models;
using HF.Api.Services;

namespace HF.Tests;

public class RegistrationRulesTests
{
    private static Event CreateEvent(
        int capacity = 10,
        int daysFromNow = 7)
    {
        return new Event
        {
            Id = 1,
            Name = "Testni dogodek",
            Location = "Ljubljana",
            Capacity = capacity,
            DateTime = DateTime.UtcNow.AddDays(daysFromNow)
        };
    }

    [Fact]
    public void FullEvent_RegistrationIsRejected()
    {
        var ev = CreateEvent(capacity: 10);

        var result = RegistrationRules.Validate(
            ev,
            registeredCount: 10,
            alreadyRegistered: false,
            utcNow: DateTime.UtcNow);

        Assert.Equal(RegistrationError.EventFull, result);
    }

    [Fact]
    public void DuplicateEmail_RegistrationIsRejected()
    {
        var ev = CreateEvent();

        var result = RegistrationRules.Validate(
            ev,
            registeredCount: 1,
            alreadyRegistered: true,
            utcNow: DateTime.UtcNow);

        Assert.Equal(RegistrationError.DuplicateEmail, result);
    }

    [Fact]
    public void PastEvent_RegistrationIsRejected()
    {
        var ev = CreateEvent(daysFromNow: -7);

        var result = RegistrationRules.Validate(
            ev,
            registeredCount: 0,
            alreadyRegistered: false,
            utcNow: DateTime.UtcNow);

        Assert.Equal(RegistrationError.PastEvent, result);
    }

    [Fact]
    public void ValidRegistration_IsAllowed()
    {
        var ev = CreateEvent(capacity: 10);

        var result = RegistrationRules.Validate(
            ev,
            registeredCount: 5,
            alreadyRegistered: false,
            utcNow: DateTime.UtcNow);

        Assert.Equal(RegistrationError.None, result);
    }

    [Fact]
    public void EmailNormalization_IsCaseInsensitive()
    {
        var email = "  ANA.NOVAK@EXAMPLE.COM  ";

        var normalized = RegistrationRules.NormalizeEmail(email);

        Assert.Equal("ana.novak@example.com", normalized);
    }

    [Fact]
    public void EventAtExactStartTime_IsPast()
    {
        var now = DateTime.UtcNow;
        var ev = CreateEvent();
        ev.DateTime = now;

        var result = RegistrationRules.Validate(
            ev,
            registeredCount: 0,
            alreadyRegistered: false,
            utcNow: now);

        Assert.Equal(RegistrationError.PastEvent, result);
    }
}
