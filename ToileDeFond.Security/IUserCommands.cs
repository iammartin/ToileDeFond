using System.Web.Security;

namespace ToileDeFond.Security
{
    public interface IUserCommands
    {
        /// <summary>
        /// Register a new account.
        /// </summary>
        /// <param name="account">Acount to register</param>
        /// <returns>Result indication</returns>
        /// <remarks>
        /// Implementations should set the <see cref="IUser.ProviderUserKey"/> property before returning.
        /// </remarks>
        MembershipCreateStatus Register(IUser user);

        /// <summary>
        /// Update an existing user.
        /// </summary>
        /// <param name="account">Account being updated.</param>
        void Update(IUser account);

        /// <summary>
        /// Delete a user from the database.
        /// </summary>
        /// <param name="username">Unique user name</param>
        /// <param name="deleteAllRelatedData">Delete information from all other tables etc</param>
        /// <returns>true if was removed successfully; otherwise false.</returns>
        bool Delete(string username, bool deleteAllRelatedData);

        IUser CreateNewUserWithTemporaryRandomName(IUser user);
    }
}