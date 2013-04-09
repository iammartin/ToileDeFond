using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using ToileDeFond.Security.PasswordStrategies;
using Microsoft.Web.WebPages.OAuth;

namespace ToileDeFond.Security
{
    public interface IUserService
    {
        string Description { get; }
        IPasswordPolicy PasswordPolicy { get; }
        IPasswordStrategy PasswordStrategy { get; }
        IUserQueries UserQueries { get; }
        IUserCommands UserCommands { get; }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to retrieve their passwords.
        /// </summary>
        /// <returns>
        /// true if the membership provider is configured to support password retrieval; otherwise, false. The default is false.
        /// </returns>
        bool EnablePasswordRetrieval { get; }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to reset their passwords.
        /// </summary>
        /// <returns>
        /// true if the membership provider supports password reset; otherwise, false. The default is true.
        /// </returns>
        bool EnablePasswordReset { get; }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
        /// </summary>
        /// <returns>
        /// true if a password answer is required for password reset and retrieval; otherwise, false. The default is true.
        /// </returns>
        bool RequiresQuestionAndAnswer { get; }

        /// <summary>
        /// The name of the application using the custom membership provider.
        /// </summary>
        /// <returns>
        /// The name of the application using the custom membership provider.
        /// </returns>
        string ApplicationName { get; set; }

        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </summary>
        /// <returns>
        /// The number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </returns>
        int MaxInvalidPasswordAttempts { get; }

        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
        /// <returns>
        /// The number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </returns>
        int PasswordAttemptWindow { get; }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
        /// </summary>
        /// <returns>
        /// true if the membership provider requires a unique e-mail address; otherwise, false. The default is true.
        /// </returns>
        bool RequiresUniqueEmail { get; }

        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Web.Security.MembershipPasswordFormat"/> values indicating the format for storing passwords in the data store.
        /// </returns>
        MembershipPasswordFormat PasswordFormat { get; }

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        /// <returns>
        /// The minimum length required for a password. 
        /// </returns>
        int MinRequiredPasswordLength { get; }

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        /// <returns>
        /// The minimum number of special characters that must be present in a valid password.
        /// </returns>
        int MinRequiredNonAlphanumericCharacters { get; }

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        /// <returns>
        /// A regular expression used to evaluate a password.
        /// </returns>
        string PasswordStrengthRegularExpression { get; }

        /// <summary>
        /// Gets the collection of all registered authentication client;
        /// </summary>
        /// <returns></returns>
        ICollection<AuthenticationClientData> OAuthRegisteredClientData { get; }



        bool ChangePassword(string username, string oldPassword, string newPassword);

        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <returns>
        /// true if the specified username and password are valid; otherwise, false.
        /// </returns>
        /// <param name="username">The name of the user to validate. </param><param name="password">The password for the specified user. </param>
        bool ValidateUser(string username, string password);

        IUser GetCurrentUserWithHttpScopeCaching(IPrincipal user, bool userIsOnline);

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.IUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        /// <param name="username">The name of the user to get information for. </param><param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user. </param>
        IUser GetUser(string username, bool userIsOnline);

       

        bool Login(string userName, string password, bool persistCookie = false);
        void Logout();
        string OAuthGetUserName(string provider, string providerUserId);
        bool OAuthHasLocalAccount(string userName);
        ICollection<OAuthAccount> OAuthGetAccountsFromUserName(string name);
        void OAuthDeleteAccount(string provider, string providerUserId);
        string GetUserId(string name);
        AuthenticationResult OAuthVerifyAuthentication(string action);
        string SerializeProviderUserId(string provider, string providerUserId);
        AuthenticationClientData OAuthGetOAuthClientData(string provider);
        bool OAuthTryDeserializeProviderUserId(string externalLoginData, out string provider, out string providerUserId);

        /// <summary>
        /// Requests the specified provider to start the authentication by directing users to an external website
        /// </summary>
        /// <param name="provider">The provider.</param>
        void OAuthRequestAuthentication(string provider);

        /// <summary>
        /// Requests the specified provider to start the authentication by directing users to an external website
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="returnUrl">The return url after user is authenticated.</param>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "We want to allow relative app path, and support ~/")]
        void OAuthRequestAuthentication(string provider, string returnUrl);

        /// <summary>
        /// Checks if the specified provider user id represents a valid account.
        /// If it does, log user in.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="providerUserId">The provider user id.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> create persistent cookie as part of the login.</param>
        /// <returns>
        ///   <c>true</c> if the login is successful.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "Login is used more consistently in ASP.Net")]
        bool OAuthLogin(string providerName, string providerUserId, bool createPersistentCookie = false);

        IUser CreateOrUpdateUser(IUser user);
    }
}