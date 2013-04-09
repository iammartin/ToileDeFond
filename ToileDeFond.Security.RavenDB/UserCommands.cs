using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Security;
using ToileDeFond.Modularity;
using Raven.Client;
using Raven.Database.Exceptions;

namespace ToileDeFond.Security.RavenDB
{
    [PrioritisedExport(typeof(IUserCommands))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserCommands : IUserCommands
    {
        private readonly IDocumentSession _documentSession;

        [ImportingConstructor]
        public UserCommands(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public IDocumentSession DocumentSession
        {
            get { return _documentSession; }
        }

        public bool Delete(string username, bool deleteAllRelatedData)
        {
            var dbUser = DocumentSession.Query<User>().SingleOrDefault(user => user.Name == username);

            if (dbUser == null)
                return true;

            DocumentSession.Delete(dbUser);
            DocumentSession.SaveChanges();

            return true;
        }

        #region Faire une method avec tout ça?

        public MembershipCreateStatus Register(IUser user)
        {
            DocumentSession.Store(user as User);
            DocumentSession.SaveChanges();

            return MembershipCreateStatus.Success;
        }

        //TODO: Quand y'a des savechanges a meme le service s'assurer de ne pas utiliser un documentsession partagé comme ici... se fair eune nouvelle session!!!
        public void Update(IUser user)
        {
            DocumentSession.Store(user as User);
            DocumentSession.SaveChanges();
        }

        //Used for third party authentication
        public IUser CreateNewUserWithTemporaryRandomName(IUser user)
        {
            const int numberOfTries = 5;
            var tryNumber = 0;
            var tryAgain = true;

            do
            {
                try
                {
                    //user.Name = "user" + 6.RandomString();

                    var random = new Random(DateTime.Now.Millisecond);
                    var randomNumber = random.Next(1, 5000000);
                    user.Name = "user" + randomNumber;

                    DocumentSession.Store(user);
                    DocumentSession.SaveChanges();
                    tryAgain = false;
                }
                catch (OperationVetoedException)
                {
                    tryNumber++;
                }
            } while (tryAgain && tryNumber < numberOfTries);

            return user;
        }

        #endregion
    }
}