namespace ToileDeFond.Security
{
    /// <summary>
    /// Extension methods for <seealso cref="IUser"/>
    /// </summary>
    public static class PasswordExtensions
    {
        /// <summary>
        /// Create a new password info class.
        /// </summary>
        /// <param name="account">Account containing password information</param>
        /// <returns>Password info object</returns>
        public static AccountPasswordInfo CreatePasswordInfo(this IUser account)
        {
            return new AccountPasswordInfo(account.Name, account.Password) { PasswordSalt = account.PasswordSalt };
        }
    }
}
