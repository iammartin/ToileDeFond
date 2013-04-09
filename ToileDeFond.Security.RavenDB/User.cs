using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToileDeFond.Security.RavenDB
{
    public class User : IUser
    {
        public User()
        {
            _thirdPartyAuthenticationUserAccounts = new List<IThirdPartyAuthenticationUserAccount>();
        }

        public static readonly IUser Anonymous = new User
        {
            Name = "Anonymous"
        };

        //TODO: Voir si ca marche les private readonly avec RavenDB
        private readonly List<IThirdPartyAuthenticationUserAccount> _thirdPartyAuthenticationUserAccounts;

        public string Name { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public List<IThirdPartyAuthenticationUserAccount> ThirdPartyAuthenticationUserAccounts
        {
            get { return _thirdPartyAuthenticationUserAccounts; }
        }

        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? FailedPasswordWindowStartedAt { get; set; }
        public int FailedPasswordWindowAttemptCount { get; set; }
        public DateTime? FailedPasswordAnswerWindowStartedAt { get; set; }
        public DateTime? LastLockedOutAt { get; set; }
        public DateTime? LastActivityAt { get; set; }
        public bool IsOnline { get; set; }
        public string Id { get; set; }
    }
}
