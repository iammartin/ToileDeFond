using System;
using System.Collections.Generic;

namespace ToileDeFond.Security
{
    public interface IUser
    {
        string Name { get; set; }
        string Password { get; set; }
        string PasswordSalt { get; set; }
         List<IThirdPartyAuthenticationUserAccount> ThirdPartyAuthenticationUserAccounts { get; }
        string Email { get; set; }
        DateTime CreatedAt { get; set; }
        bool IsLockedOut { get; set; }
        DateTime? LastLoginAt { get; set; }
        DateTime? FailedPasswordWindowStartedAt { get; set; }
        int FailedPasswordWindowAttemptCount { get; set; }
        DateTime? FailedPasswordAnswerWindowStartedAt { get; set; }
        DateTime? LastLockedOutAt { get; set; }
        DateTime? LastActivityAt { get; set; }
        bool IsOnline { get; set; }
        string Id { get; set; }
    }
}