using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ToileDeFond.Modularity;
using Raven.Client;

namespace ToileDeFond.Security.RavenDB
{
    [PrioritisedExport(typeof(IUserQueries))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserQueries : IUserQueries
    {
        private readonly IDocumentSession _documentSession;

        [ImportingConstructor]
        public UserQueries(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        ///// <summary>
        /////     Check if a user exists by user token.
        ///// </summary>
        //public bool AnyByUserToken(IDocumentSession session, string token)
        //{
        //    return session.Query<User, UserProfileByUserId>().Any(p => p.FacebookUserId == token);
        //}

        ///// <summary>
        /////     Load a user by user token.
        ///// </summary>
        //public User GetByUserToken(IDocumentSession session, string token)
        //{
        //    return session.Query<User, UserProfileByUserId>().FirstOrDefault(p => p.FacebookUserId == token);
        //}

        ///// <summary>
        /////     Check if a user exists by user id.
        ///// </summary>
        //public bool AnyByUserId(IDocumentSession session, long userId)
        //{
        //    return session.Load<Us>()
        //}

        ///// <summary>
        /////     Load a user by user id.
        ///// </summary>
        //public User GetByUserId(IDocumentSession session, long userId)
        //{
        //    return session.Query<User, UserProfileByUserId>().FirstOrDefault(p => p.Id == userId);
        //}

        public IDocumentSession DocumentSession
        {
            get { return _documentSession; }
        }

        /// <summary>
        /// Fetch a user from the service.
        /// </summary>
        /// <param name="username">Unique user name</param>
        /// <returns>User if found; otherwise null.</returns>
        public IUser Get(string username)
        {

            return DocumentSession.Query<User>().FirstOrDefault(user => user.Name == username);

        }

        ///// <summary>
        ///// Get a user by using the implementation specific (your) Id.
        ///// </summary>
        ///// <param name="id">User identity specific for each account repository implementation</param>
        ///// <returns>User if found; otherwise null.</returns>
        //public User GetByProviderKey(object id)
        //{
        //    if (id == null) throw new ArgumentNullException("id");


        //    return DocumentSession.Query<User>().FirstOrDefault(user => user.ProviderUserKey == id);

        //}

        /// <summary>
        /// Translate an email into a user name.
        /// </summary>
        /// <param name="email">Email to lookup</param>
        /// <returns>User name if the specified email was found; otherwise null.</returns>
        public string GetUserNameByEmail(string email)
        {
            if (email == null) throw new ArgumentNullException("email");


            return DocumentSession.Query<User>().Where(user => user.Email == email).Select(user => user.Name).FirstOrDefault();

        }

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
        //    //    return DocumentSession.Query<User>().Count(user => user.IsOnline);
        //    //}
        //}

        /// <summary>
        /// Find all users
        /// </summary>
        /// <param name="pageIndex">One based index</param>
        /// <param name="pageSize">Number of users per page</param>
        /// <param name="totalRecords">Total number of users</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        public IEnumerable<IUser> FindAll(int pageIndex, int pageSize, out int totalRecords)
        {
            IQueryable<User> query;


            query = DocumentSession.Query<User>();


            query = CountAndPageQuery(pageIndex, pageSize, out totalRecords, query);

            return query.ToList();
        }

        /// <summary>
        /// Find new acounts that haven't been activated.
        /// </summary>
        /// <param name="pageIndex">zero based index</param>
        /// <param name="pageSize">Number of users per page</param>
        /// <param name="totalRecords">Total number of users</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        public IEnumerable<IUser> FindNewAccounts(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
            //IQueryable<User> query;


            //query = DocumentSession.Query<User>().Where(p => p.IsApproved == false);


            //query = CountAndPageQuery(pageIndex, pageSize, out totalRecords, query);

            //return query.ToList();
        }

        /// <summary>
        /// Find by searching for user name
        /// </summary>
        /// <param name="usernameToMatch">User name (or partial user name)</param>
        /// <param name="pageIndex">Zero based index</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="totalRecords">total number of records that partially matched the specified user name</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        public IEnumerable<IUser> FindByUserName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            IQueryable<User> query;

            query = DocumentSession.Query<User>().Where(user => user.Name.Contains(usernameToMatch));


            query = CountAndPageQuery(pageIndex, pageSize, out totalRecords, query);
            return query.ToList();
        }

        /// <summary>
        /// Find by searching for the specified email
        /// </summary>
        /// <param name="emailToMatch">Number of users that have the specified email (no partial matches)</param>
        /// <param name="pageIndex">Zero based index</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="totalRecords">total number of records that matched the specified email</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        public IEnumerable<IUser> FindByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            IQueryable<User> query;

            query = DocumentSession.Query<User>().Where(user => user.Email == emailToMatch);


            query = CountAndPageQuery(pageIndex, pageSize, out totalRecords, query);
            return query.ToList();
        }

        private IQueryable<User> CountAndPageQuery(int pageIndex, int pageSize, out int totalRecords, IQueryable<User> query)
        {
            totalRecords = query.Count();

            query = pageIndex == 1
                        ? DocumentSession.Query<User>().Take(pageSize)
                        : DocumentSession.Query<User>().Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return query;
        }

        //public User GetUserById(long id)
        //{
        //    return GetUserById("users/" + id);
        //}

        /// <summary>
        ///     Load post by encoded string id.
        /// </summary>
        //public User GetUserById(string id)
        //{
        //    return DocumentSession.Load<User>(id);
        //}


        #region OAuth


        public IUser GetUserByThirdPartyAuthentication(string thirdPartyAuthenticationName, string thirdPartyAuthenticationId)
        {
            return DocumentSession.Query<IUser>().FirstOrDefault(p =>
                p.ThirdPartyAuthenticationUserAccounts.Any(a => a.Id == thirdPartyAuthenticationId &&
                                                                a.Name == thirdPartyAuthenticationName));
        }

        #endregion
    }
}