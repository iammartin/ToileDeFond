using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using ToileDeFond.Modularity;
using ToileDeFond.Security.PasswordStrategies;
using ToileDeFond.Utilities;
using Microsoft.Web.WebPages.OAuth;

namespace ToileDeFond.Security
{
    [PrioritisedExport(typeof(IUserService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserService : IUserService, IOpenAuthDataProvider
    {
        private readonly IUserCommands _userCommands;
        private readonly IUserQueries _userQueries;
        private readonly IPasswordStrategy _passwordStrategy;
        private readonly IPasswordPolicy _passwordPolicy;
        private readonly IOAuthRegisteredClientCollection _oAuthRegisteredClientCollection;

        //TODO: Faire une interface web pour ordonner les IPrioritisedMefMetaData - très bon test avec le IPasswordStrategy

        [ImportingConstructor]
        public UserService(IUserCommands userCommands, IUserQueries userQueries,  [ImportMany]IEnumerable<Lazy<IPasswordStrategy, IPrioritisedMefMetaData>>  passwordStrategies,
            IPasswordPolicy passwordPolicy, IOAuthRegisteredClientCollection oAuthRegisteredClientCollection)
        {
            _userCommands = userCommands;
            _userQueries = userQueries;
            _passwordStrategy = passwordStrategies.OrderByDescending(x => x.Metadata.Priority).First().Value;
            _passwordPolicy = passwordPolicy;
            _oAuthRegisteredClientCollection = oAuthRegisteredClientCollection;
        }

        #region Old Membership Provider

        public string Description
        {
            get { return "A more friendly membership provider."; }
        }

        public IPasswordPolicy PasswordPolicy
        {
            get { return _passwordPolicy; }
        }

        public IPasswordStrategy PasswordStrategy
        {
            get { return _passwordStrategy; }
        }

        public IUserQueries UserQueries
        {
            get { return _userQueries; }
        }

        public IUserCommands UserCommands
        {
            get { return _userCommands; }
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to retrieve their passwords.
        /// </summary>
        /// <returns>
        /// true if the membership provider is configured to support password retrieval; otherwise, false. The default is false.
        /// </returns>
        public bool EnablePasswordRetrieval
        {
            get { return PasswordStrategy.IsPasswordsDecryptable && PasswordPolicy.IsPasswordRetrievalEnabled; }
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to reset their passwords.
        /// </summary>
        /// <returns>
        /// true if the membership provider supports password reset; otherwise, false. The default is true.
        /// </returns>
        public bool EnablePasswordReset
        {
            get { return PasswordPolicy.IsPasswordResetEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
        /// </summary>
        /// <returns>
        /// true if a password answer is required for password reset and retrieval; otherwise, false. The default is true.
        /// </returns>
        public bool RequiresQuestionAndAnswer
        {
            get { return PasswordPolicy.IsPasswordQuestionRequired; }
        }

        /// <summary>
        /// The name of the application using the custom membership provider.
        /// </summary>
        /// <returns>
        /// The name of the application using the custom membership provider.
        /// </returns>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </summary>
        /// <returns>
        /// The number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </returns>
        public int MaxInvalidPasswordAttempts
        {
            get { return PasswordPolicy.MaxInvalidPasswordAttempts; }
        }

        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
        /// <returns>
        /// The number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </returns>
        public int PasswordAttemptWindow
        {
            get { return PasswordPolicy.PasswordAttemptWindow; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
        /// </summary>
        /// <returns>
        /// true if the membership provider requires a unique e-mail address; otherwise, false. The default is true.
        /// </returns>
        public bool RequiresUniqueEmail
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Web.Security.MembershipPasswordFormat"/> values indicating the format for storing passwords in the data store.
        /// </returns>
        public MembershipPasswordFormat PasswordFormat
        {
            get { return PasswordStrategy.PasswordFormat; }
        }

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        /// <returns>
        /// The minimum length required for a password. 
        /// </returns>
        public int MinRequiredPasswordLength
        {
            get { return PasswordPolicy.PasswordMinimumLength; }
        }

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        /// <returns>
        /// The minimum number of special characters that must be present in a valid password.
        /// </returns>
        public int MinRequiredNonAlphanumericCharacters
        {
            get { return PasswordPolicy.MinRequiredNonAlphanumericCharacters; }
        }

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        /// <returns>
        /// A regular expression used to evaluate a password.
        /// </returns>
        public string PasswordStrengthRegularExpression
        {
            get { return PasswordPolicy.PasswordStrengthRegularExpression; }
        }

        public ICollection<AuthenticationClientData> OAuthRegisteredClientData
        {
            get { return _oAuthRegisteredClientCollection.OAuthRegisteredClientData; }
        }

        //Demander un unique email... e préremplir du provider si Oauth et disponible... (exemple Facebook).. comme il semble faire pour le username (a voir)

        public IUser CreateOrUpdateUser(IUser user)
        {
            if (String.IsNullOrEmpty(user.Name)) throw new MembershipCreateUserException(MembershipCreateStatus.InvalidUserName);
            //if (String.IsNullOrEmpty(user.Password)) throw new MembershipCreateUserException(MembershipCreateStatus.InvalidPassword);
            if (String.IsNullOrEmpty(user.Email)) throw new MembershipCreateUserException(MembershipCreateStatus.InvalidEmail);

            if (user.Id.IsNullOrEmpty()) // New user...
            {
                if (UserQueries.GetUserNameByEmail(user.Email) != null)
                {
                    throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateEmail);
                }

                if (UserQueries.Get(user.Name) != null)
                {
                    throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateUserName);
                }

                user.CreatedAt = DateTime.Now;
            }

            if (!user.ThirdPartyAuthenticationUserAccounts.Any() || !user.Password.IsNullOrEmpty())
            {
                try
                {
                    ValidatePassword(user.Name, user.Password);
                }
                catch
                {
                    // not the smoothest approach, but the best 
                    // considering the inconsistent password failure handling.
                    throw new MembershipCreateUserException(MembershipCreateStatus.InvalidPassword);
                }
            }

            var passwordInfo = new AccountPasswordInfo(user.Name, user.Password);
            user.Password = PasswordStrategy.Encrypt(passwordInfo);
            user.PasswordSalt = passwordInfo.PasswordSalt;

            var status = UserCommands.Register(user);

            if (status != MembershipCreateStatus.Success)
                throw new MembershipCreateUserException(status);

            return user;
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(username)) throw new ArgumentException("Value cannot be null or empty.", "username");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {


                var account = UserQueries.Get(username);
                var pwInfo = account.CreatePasswordInfo();
                if (!PasswordStrategy.Compare(pwInfo, oldPassword))
                    return false;

                ValidatePassword(username, newPassword);

                account.Password = newPassword;
                pwInfo = account.CreatePasswordInfo();
                account.Password = PasswordStrategy.Encrypt(pwInfo);
                UserCommands.Update(account);

                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }

        ///// <summary>
        ///// Processes a request to update the password question and answer for a membership user.
        ///// </summary>
        ///// <returns>
        ///// true if the password question and answer are updated successfully; otherwise, false.
        ///// </returns>
        ///// <param name="username">The user to change the password question and answer for. </param><param name="password">The password for the specified user. </param><param name="newPasswordQuestion">The new password question for the specified user. </param><param name="newPasswordAnswer">The new password answer for the specified user. </param>
        //public  bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        //{
        //    var account = UserQueries.Get(username);
        //    if (account == null)
        //        return false;

        //    var info = new AccountPasswordInfo(username, account.Password)
        //                   {
        //                       PasswordSalt = account.PasswordSalt
        //                   };
        //    if (PasswordStrategy.Compare(info, password))
        //        return false;

        //    account.PasswordQuestion = newPasswordAnswer;
        //    account.PasswordAnswer = newPasswordAnswer;
        //    AccountRepository.Update(account);
        //    return true;
        //}

        ///// <summary>
        ///// Gets the password for the specified user name from the data source.
        ///// </summary>
        ///// <returns>
        ///// The password for the specified user name.
        ///// </returns>
        ///// <param name="username">The user to retrieve the password for. </param><param name="answer">The password answer for the user. </param>
        //public  string GetPassword(string username, string answer)
        //{
        //    if (!PasswordPolicy.IsPasswordRetrievalEnabled || !PasswordStrategy.IsPasswordsDecryptable)
        //        throw new ProviderException("Password retrieval is not supported");

        //    var account = UserQueries.Get(username);
        //    if (!account.PasswordAnswer.Equals(answer, StringComparison.OrdinalIgnoreCase))
        //        throw new MembershipPasswordException("Answer to Password question was incorrect.");

        //    return PasswordStrategy.Decrypt(account.Password);
        //}


        ///// <summary>
        ///// Resets a user's password to a new, automatically generated password.
        ///// </summary>
        ///// <returns>
        ///// The new password for the specified user.
        ///// </returns>
        ///// <param name="username">The user to reset the password for. </param><param name="answer">The password answer for the specified user. </param>
        //public  string ResetPassword(string username, string answer)
        //{
        //    if (!PasswordPolicy.IsPasswordResetEnabled)
        //        throw new NotSupportedException("Password reset is not supported.");

        //    var user = UserQueries.Get(username);
        //    if (PasswordPolicy.IsPasswordQuestionRequired && answer == null)
        //        throw new MembershipPasswordException("Password answer is empty and question/answer is required.");

        //    if (!user.PasswordAnswer.Equals(answer, StringComparison.OrdinalIgnoreCase))
        //        return null;

        //    var newPassword = PasswordStrategy.GeneratePassword(PasswordPolicy);

        //    ValidatePassword(username, newPassword);

        //    var info = new AccountPasswordInfo(username, newPassword);
        //    user.Password = PasswordStrategy.Encrypt(info);
        //    user.PasswordSalt = info.PasswordSalt;
        //    AccountRepository.Update(user);
        //    return newPassword;
        //}

        private void ValidatePassword(string username, string clearTextPassword)
        {
            if (!PasswordStrategy.IsValid(clearTextPassword, PasswordPolicy))
                throw new MembershipPasswordException("Password failed validation");

            var args = new ValidatePasswordEventArgs(username, clearTextPassword, false);

            if (args.FailureInformation != null)
                throw args.FailureInformation;
        }

        ///// <summary>
        ///// Updates information about a user in the data source.
        ///// </summary>
        ///// <param name="user">A <see cref="T:System.Web.Security.IUser"/> object that represents the user to update and the updated information for the user. </param>
        //public  void UpdateUser(IUser user)
        //{
        //    var account = UserQueries.Get(user.UserName);
        //    Merge(user, account);
        //    AccountRepository.Update(account);
        //}

        //private void Merge(IUser user, IUser account)
        //{
        //    account.Comment = user.Comment;
        //    account.IsApproved = user.IsApproved;
        //    account.Email = user.Email;
        //    account.PasswordQuestion = user.PasswordQuestion;
        //    account.IsLockedOut = user.IsLockedOut;
        //    //account.IsOnline = user.IsOnline;
        //    account.LastActivityAt = user.LastActivityDate;
        //    account.LastLockedOutAt = user.LastLockoutDate;
        //    account.LastPasswordChangeAt = user.LastPasswordChangedDate;
        //    account.ProviderUserKey = user.ProviderUserKey;
        //    account.Name = user.UserName;
        //}

        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <returns>
        /// true if the specified username and password are valid; otherwise, false.
        /// </returns>
        /// <param name="username">The name of the user to validate. </param><param name="password">The password for the specified user. </param>
        public bool ValidateUser(string username, string password)
        {
            if (String.IsNullOrEmpty(username)) throw new ArgumentException("Value cannot be null or empty.", "username");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            var user = UserQueries.Get(username);
            if (user == null || user.IsLockedOut)
                return false;

            var passwordInfo = user.CreatePasswordInfo();
            var validated = PasswordStrategy.Compare(passwordInfo, password);
            if (validated)
            {
                user.LastLoginAt = DateTime.Now;
                user.FailedPasswordWindowStartedAt = null;
                user.FailedPasswordWindowAttemptCount = 0;
                UserCommands.Update(user);
                return true;
            }

            user.FailedPasswordWindowAttemptCount += 1;

            if (!user.FailedPasswordWindowStartedAt.HasValue)
                user.FailedPasswordAnswerWindowStartedAt = DateTime.Now;
            else if (DateTime.Now.Subtract(user.FailedPasswordAnswerWindowStartedAt.Value).TotalMinutes >
                     PasswordPolicy.PasswordAttemptWindow)
            {
                user.IsLockedOut = true;
                user.LastLockedOutAt = DateTime.Now;
                UserCommands.Update(user);
            }

            return false;
        }

        ///// <summary>
        ///// Clears a lock so that the membership user can be validated.
        ///// </summary>
        ///// <returns>
        ///// true if the membership user was successfully unlocked; otherwise, false.
        ///// </returns>
        ///// <param name="userName">The membership user whose lock status you want to clear.</param>
        //public  bool UnlockUser(string userName)
        //{
        //    var user = UserQueries.Get(userName);
        //    if (user == null)
        //        return false;

        //    user.IsLockedOut = false;
        //    user.FailedPasswordAnswerWindowAttemptCount = 0;
        //    user.FailedPasswordAnswerWindowStartedAt = DateTime.MinValue;
        //    user.FailedPasswordWindowAttemptCount = 0;
        //    user.FailedPasswordWindowStartedAt = DateTime.MinValue;
        //    AccountRepository.Update(user);
        //    return true;
        //}

        ///// <summary>
        ///// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        ///// </summary>
        ///// <returns>
        ///// A <see cref="T:System.Web.Security.IUser"/> object populated with the specified user's information from the data source.
        ///// </returns>
        ///// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param><param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        //public IUser GetUser(object providerUserKey, bool userIsOnline)
        //{
        //    var user = UserQueries.GetByProviderKey(providerUserKey);

        //    if (user == null)
        //        return null;

        //    UpdateOnlineState(userIsOnline, user);

        //    return user;
        //}

        private IUser _currentUser;
        public IUser GetCurrentUserWithHttpScopeCaching(IPrincipal user, bool userIsOnline)
        {
            return _currentUser ?? (_currentUser = (user.Identity.Name.IsNullOrEmpty() ? IUser.Anonymous : GetUser(user.Identity.Name, userIsOnline) ?? IUser.Anonymous));
        }

        private void UpdateOnlineState(bool userIsOnline, IUser user)
        {
            if (!userIsOnline)
                return;

            user.LastActivityAt = DateTime.Now;
            user.IsOnline = true;

            UserCommands.Update(user);
        }

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.IUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        /// <param name="username">The name of the user to get information for. </param><param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user. </param>
        public IUser GetUser(string username, bool userIsOnline)
        {
            var user = UserQueries.Get(username);
            if (user == null)
                return null;

            UpdateOnlineState(userIsOnline, user);

            return user;
        }

        #endregion

        #region New Membership Provider


        #region RequestAuthentication


        /// <summary>
        /// Requests the specified provider to start the authentication by directing users to an external website
        /// </summary>
        /// <param name="provider">The provider.</param>
        public void OAuthRequestAuthentication(string provider)
        {
            OAuthRequestAuthentication(provider, returnUrl: null);
        }

        /// <summary>
        /// Requests the specified provider to start the authentication by directing users to an external website
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="returnUrl">The return url after user is authenticated.</param>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "We want to allow relative app path, and support ~/")]
        public void OAuthRequestAuthentication(string provider, string returnUrl)
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("HttpContextNotAvailable");
            }

            OAuthRequestAuthenticationCore(new HttpContextWrapper(HttpContext.Current), provider, returnUrl);
        }

        /// <summary>
        /// Requests the authentication core.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="returnUrl">The return URL.</param>
        protected virtual void OAuthRequestAuthenticationCore(HttpContextBase context, string provider, string returnUrl)
        {
            IAuthenticationClient client = _oAuthRegisteredClientCollection.GetOAuthClient(provider);
            var securityManager = new OpenAuthSecurityManager(context, client, this);
            securityManager.RequestAuthentication(returnUrl);
        }

        #endregion


        #region VerifyAuthentication

        /// <summary>
        /// Checks if user is successfully authenticated when user is redirected back to this user.
        /// </summary>
        [CLSCompliant(false)]
        public AuthenticationResult OAuthVerifyAuthentication()
        {
            return OAuthVerifyAuthentication(returnUrl: null);
        }

        /// <summary>
        /// Checks if user is successfully authenticated when user is redirected back to this user.
        /// </summary>
        /// <param name="returnUrl">The return URL which must match the one passed to RequestAuthentication earlier.</param>
        [CLSCompliant(false)]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "We want to allow relative app path, and support ~/")]
        public AuthenticationResult OAuthVerifyAuthentication(string returnUrl)
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("HttpContextNotAvailable");
            }

            return OAuthVerifyAuthenticationCore(new HttpContextWrapper(HttpContext.Current), returnUrl);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "We want to allow relative app path, and support ~/")]
        protected virtual AuthenticationResult OAuthVerifyAuthenticationCore(HttpContextBase context, string returnUrl)
        {
            string providerName = OpenAuthSecurityManager.GetProviderName(context);
            if (String.IsNullOrEmpty(providerName))
            {
                return AuthenticationResult.Failed;
            }

            IAuthenticationClient client;
            if (_oAuthRegisteredClientCollection.TryGetOAuthClient(providerName, out client))
            {
                var securityManager = new OpenAuthSecurityManager(context, client, this);
                return securityManager.VerifyAuthentication(returnUrl);
            }
            else
            {
                throw new InvalidOperationException("InvalidServiceProviderName");
            }
        }

        #endregion

        #region Login

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
        public bool OAuthLogin(string providerName, string providerUserId, bool createPersistentCookie)
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("HttpContextNotAvailable");
            }

            return OAuthLoginCore(new HttpContextWrapper(HttpContext.Current), providerName, providerUserId, createPersistentCookie);
        }

        //public void CreateOrUpdateUser(IUser user)
        //{
        //    // Insert a new user into the database
        //    using (UsersContext db = new UsersContext())
        //    {
        //        UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
        //        // Check if user already exists
        //        if (user == null)
        //        {
        //            // Insert name into the profile table
        //            db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
        //            db.SaveChanges();

        //            UserService.OAuthCreateOrUpdateAccount(provider, providerUserId, model.UserName);
        //            UserService.OAuthLogin(provider, providerUserId);

        //            return RedirectToLocal(returnUrl);
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("UserName", "IUser name already exists. Please enter a different user name.");
        //        }
        //    }
        //}

        protected virtual bool OAuthLoginCore(HttpContextBase context, string providerName, string providerUserId, bool createPersistentCookie)
        {
            var provider = _oAuthRegisteredClientCollection.GetOAuthClient(providerName);
            var securityManager = new OpenAuthSecurityManager(context, provider, this);
            return securityManager.Login(providerUserId, createPersistentCookie);
        }

        #endregion


        public bool Login(string userName, string password, bool persistCookie = false)
        {
            bool success = ValidateUser(userName, password);
            if (success)
            {
                FormsAuthentication.SetAuthCookie(userName, persistCookie);
            }
            return success;
        }

        public void Logout()
        {
            FormsAuthentication.SignOut();
        }

        public string OAuthGetUserName(string provider, string providerUserId)
        {
            throw new NotImplementedException();
        }

        public bool OAuthHasLocalAccount(string userName)
        {
            var user = UserQueries.Get(userName);

            return !user.Password.IsNullOrEmpty();
        }

        public ICollection<OAuthAccount> OAuthGetAccountsFromUserName(string userName)
        {
             if (String.IsNullOrEmpty(userName))
            {
                throw new ArgumentException(
                    String.Format(CultureInfo.CurrentCulture, "Argument_Cannot_Be_Null_Or_Empty", "userName"),
                    "userName");
            }

            var user = UserQueries.Get(userName);

            return user.ThirdPartyAuthenticationUserAccounts.Select(p => new OAuthAccount(p.Name, p.Id)).ToList();
        }

        public void OAuthDeleteAccount(string provider, string providerUserId)
        {
            throw new NotImplementedException();
        }

        public string GetUserId(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Securely serializes a providerName/providerUserId pair.
        /// </summary>
        /// <param name="providerName">The provider name.</param>
        /// <param name="providerUserId">The provider-specific user id.</param>
        /// <returns>A cryptographically protected serialization of the inputs which is suitable for round-tripping.</returns>
        /// <remarks>Do not persist the return value to permanent storage. This implementation is subject to change.</remarks>
        public string SerializeProviderUserId(string providerName, string providerUserId)
        {
            if (providerName == null)
            {
                throw new ArgumentNullException("providerName");
            }
            if (providerUserId == null)
            {
                throw new ArgumentNullException("providerUserId");
            }

            return ProviderUserIdSerializationHelper.ProtectData(providerName, providerUserId);
        }

        public AuthenticationClientData OAuthGetOAuthClientData(string provider)
        {
            return _oAuthRegisteredClientCollection.GetOAuthClientData(provider);
        }

        /// <summary>
        /// Deserializes a string obtained from <see cref="SerializeProviderUserId(string, string)"/> back into a 
        /// providerName/providerUserId pair.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <param name="providerName">Will contain the deserialized provider name.</param>
        /// <param name="providerUserId">Will contain the deserialized provider user id.</param>
        /// <returns><c>True</c> if successful.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "This design is acceptable")]
        public bool OAuthTryDeserializeProviderUserId(string data, out string providerName, out string providerUserId)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            return ProviderUserIdSerializationHelper.UnprotectData(data, out providerName, out providerUserId);
        }

        #endregion

        ///// <summary>
        ///// Gets the user name associated with the specified e-mail address.
        ///// </summary>
        ///// <returns>
        ///// The user name associated with the specified e-mail address. If no match is found, return null.
        ///// </returns>
        ///// <param name="email">The e-mail address to search for. </param>
        //public  string GetUserNameByEmail(string email)
        //{
        //    return UserQueries.GetUserNameByEmail(email);
        //}

        ///// <summary>
        ///// Removes a user from the membership data source. 
        ///// </summary>
        ///// <returns>
        ///// true if the user was successfully deleted; otherwise, false.
        ///// </returns>
        ///// <param name="username">The name of the user to delete.</param><param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
        //public  bool DeleteUser(string username, bool deleteAllRelatedData)
        //{
        //    return AccountRepository.Delete(username, deleteAllRelatedData);
        //}

        ///// <summary>
        ///// Gets a collection of all the users in the data source in pages of data.
        ///// </summary>
        ///// <returns>
        ///// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.IUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        ///// </returns>
        ///// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param><param name="pageSize">The size of the page of results to return.</param><param name="totalRecords">The total number of matched users.</param>
        //public  MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        //{
        //    var users = AccountRepository.FindAll(pageIndex, pageSize, out totalRecords);
        //    return CloneUsers(users);
        //}

        //private MembershipUserCollection CloneUsers(IEnumerable<IUser> users)
        //{
        //    var members = new MembershipUserCollection();
        //    foreach (var user in users)
        //    {
        //        members.Add(CloneUser(user));
        //    }
        //    return members;
        //}

        ///// <summary>
        ///// Gets the number of users currently accessing the application.
        ///// </summary>
        ///// <returns>
        ///// The number of users currently accessing the application.
        ///// </returns>
        //public  int GetNumberOfUsersOnline()
        //{
        //    return UserQueries.GetNumberOfUsersOnline();
        //}

        ///// <summary>
        ///// Gets a collection of membership users where the user name contains the specified user name to match.
        ///// </summary>
        ///// <returns>
        ///// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.IUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        ///// </returns>
        ///// <param name="usernameToMatch">The user name to search for.</param><param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param><param name="pageSize">The size of the page of results to return.</param><param name="totalRecords">The total number of matched users.</param>
        //public  MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
        //                                                         out int totalRecords)
        //{
        //    var users = UserQueries.FindByUserName(usernameToMatch, pageIndex, pageSize,
        //                                                 out totalRecords);
        //    return CloneUsers(users);
        //}


        ///// <summary>
        ///// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
        ///// </summary>
        ///// <returns>
        ///// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.IUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        ///// </returns>
        ///// <param name="emailToMatch">The e-mail address to search for.</param><param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param><param name="pageSize">The size of the page of results to return.</param><param name="totalRecords">The total number of matched users.</param>
        //public  MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
        //                                                          out int totalRecords)
        //{
        //    var users = UserQueries.FindByEmail(emailToMatch, pageIndex, pageSize,
        //                                              out totalRecords);
        //    return CloneUsers(users);
        //}

        #region Implementation of IOpenAuthDataProvider

        public string GetUserNameFromOpenAuth(string openAuthProvider, string openAuthId)
        {
            //TODO : Optimiser - ne pas retourner un full IUser juste pour le nom
            var user = UserQueries.GetUserByThirdPartyAuthentication(openAuthProvider, openAuthId);

            if (user == null)
                return null;

            return user.Name;
        }

        #endregion

        //Thanks for the internal class Microshit!!!
        internal static class ProviderUserIdSerializationHelper
        {
            // Custom message purpose to prevent this data from being readable by a different subsystem.
            private static byte[] _padding = new byte[] { 0x85, 0xC5, 0x65, 0x72 };

            [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "The instances are disposed correctly.")]
            public static string ProtectData(string providerName, string providerUserId)
            {
                using (MemoryStream ms = new MemoryStream())
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(providerName);
                    bw.Write(providerUserId);
                    bw.Flush();
                    byte[] serializedWithPadding = new byte[ms.Length + _padding.Length];
                    Buffer.BlockCopy(_padding, 0, serializedWithPadding, 0, _padding.Length);
                    Buffer.BlockCopy(ms.GetBuffer(), 0, serializedWithPadding, _padding.Length, (int)ms.Length);
                    return MachineKey.Encode(serializedWithPadding, MachineKeyProtection.All);
                }
            }

            [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "The instances are disposed correctly.")]
            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exception are being caught on purpose.")]
            public static bool UnprotectData(string protectedData, out string providerName, out string providerUserId)
            {
                providerName = null;
                providerUserId = null;
                if (String.IsNullOrEmpty(protectedData))
                {
                    return false;
                }

                byte[] decodedWithPadding = MachineKey.Decode(protectedData, MachineKeyProtection.All);

                if (decodedWithPadding.Length < _padding.Length)
                {
                    return false;
                }

                // timing attacks aren't really applicable to this, so we just do the simple check.
                for (int i = 0; i < _padding.Length; i++)
                {
                    if (_padding[i] != decodedWithPadding[i])
                    {
                        return false;
                    }
                }

                using (MemoryStream ms = new MemoryStream(decodedWithPadding, _padding.Length, decodedWithPadding.Length - _padding.Length))
                using (BinaryReader br = new BinaryReader(ms))
                {
                    try
                    {
                        // use temp variable to keep both out parameters consistent and only set them when the input stream is read completely
                        string name = br.ReadString();
                        string userId = br.ReadString();
                        // make sure that we consume the entire input stream
                        if (ms.ReadByte() == -1)
                        {
                            providerName = name;
                            providerUserId = userId;
                            return true;
                        }
                    }
                    catch
                    {
                        // Any exceptions will result in this method returning false.
                    }
                }
                return false;
            }
        }
    }
}
