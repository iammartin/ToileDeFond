using System.Collections.Generic;

namespace ToileDeFond.Security
{
    public interface IUserQueries
    {
        /// <summary>
        /// Fetch a user from the service.
        /// </summary>
        /// <param name="username">Unique user name</param>
        /// <returns>IUser if found; otherwise null.</returns>
        IUser Get(string username);

        ///// <summary>
        ///// Get a user by using the implementation specific (your) Id.
        ///// </summary>
        ///// <param name="id">IUser identity specific for each account repository implementation</param>
        ///// <returns>IUser if found; otherwise null.</returns>
        //IUser GetByProviderKey(object id);

        /// <summary>
        /// Translate an email into a user name.
        /// </summary>
        /// <param name="email">Email to lookup</param>
        /// <returns>IUser name if the specified email was found; otherwise null.</returns>
        string GetUserNameByEmail(string email);

        /// <summary>
        /// Get number of users that are online
        /// </summary>
        /// <returns>Number of online users</returns>
        //public int GetNumberOfUsersOnline()
        //{
        //    //Pas certai que le champ isonline est mis à jour jamais...
        //    throw new NotImplementedException();
        //    //using (var documentSession = _documentStore.OpenSession())
        //    //{
        //    //    return documentSession.Query<IUser>().Count(user => user.IsOnline);
        //    //}
        //}

        /// <summary>
        /// Find all users
        /// </summary>
        /// <param name="pageIndex">One based index</param>
        /// <param name="pageSize">Number of users per page</param>
        /// <param name="totalRecords">Total number of users</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        IEnumerable<IUser> FindAll(int pageIndex, int pageSize, out int totalRecords);

        /// <summary>
        /// Find new acounts that haven't been activated.
        /// </summary>
        /// <param name="pageIndex">zero based index</param>
        /// <param name="pageSize">Number of users per page</param>
        /// <param name="totalRecords">Total number of users</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        IEnumerable<IUser> FindNewAccounts(int pageIndex, int pageSize, out int totalRecords);

        /// <summary>
        /// Find by searching for user name
        /// </summary>
        /// <param name="usernameToMatch">IUser name (or partial user name)</param>
        /// <param name="pageIndex">Zero based index</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="totalRecords">total number of records that partially matched the specified user name</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        IEnumerable<IUser> FindByUserName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords);

        /// <summary>
        /// Find by searching for the specified email
        /// </summary>
        /// <param name="emailToMatch">Number of users that have the specified email (no partial matches)</param>
        /// <param name="pageIndex">Zero based index</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="totalRecords">total number of records that matched the specified email</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        IEnumerable<IUser> FindByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords);

        //IUser GetUserById( long id);

        ///// <summary>
        /////     Load post by encoded string id.
        ///// </summary>
        //IUser GetUserById(string id);

        IUser GetUserByThirdPartyAuthentication(string thirdPartyAuthenticationName, string thirdPartyAuthenticationId);
    }
}