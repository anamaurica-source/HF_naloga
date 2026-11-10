
using HF.Api.Models;

namespace HF.Api.Services;

public enum RegistrationError
{
    None,
    PastEvent,
    DuplicateEmail,
    EventFull
}

public static class RegistrationRules
{
    public static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    public static RegistrationError Validate(
        Event ev,
        int registeredCount,
        bool alreadyRegistered,
        DateTime utcNow)
    {
        if (ev.DateTime <= utcNow)
            return RegistrationError.PastEvent;

        if (alreadyRegistered)
            return RegistrationError.DuplicateEmail;

        if (registeredCount >= ev.Capacity)
            return RegistrationError.EventFull;

        return RegistrationError.None;
    }
}
