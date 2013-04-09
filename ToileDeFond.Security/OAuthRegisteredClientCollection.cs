using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using ToileDeFond.Modularity;
using Microsoft.Web.WebPages.OAuth;

namespace ToileDeFond.Security
{
    [PrioritisedExport(typeof(IOAuthRegisteredClientCollection))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OAuthRegisteredClientCollection : IOAuthRegisteredClientCollection
    {
        // contains all registered authentication clients
        private readonly Dictionary<string, AuthenticationClientData> _authenticationClients =
            new Dictionary<string, AuthenticationClientData>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the collection of all registered authentication client;
        /// </summary>
        /// <returns></returns>
        public ICollection<AuthenticationClientData> OAuthRegisteredClientData
        {
            get
            {
                // the Values property returns a read-only collection.
                // so we don't need to worry about clients of this method modifying our internal collection.
                return _authenticationClients.Values;
            }
        }

        #region Regiter OAuth Client

        /// <summary>
        /// Registers the Facebook client.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="appSecret">The app secret.</param>
        public void RegisterFacebookClient(string appId, string appSecret)
        {
            RegisterFacebookClient(appId, appSecret, displayName: "Facebook");
        }

        /// <summary>
        /// Registers the Facebook client.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="appSecret">The app secret.</param>
        /// <param name="displayName">The display name of the client.</param>
        public void RegisterFacebookClient(string appId, string appSecret, string displayName)
        {
            RegisterFacebookClient(appId, appSecret, displayName, extraData: new Dictionary<string, object>());
        }

        /// <summary>
        /// Registers the Facebook client.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="appSecret">The app secret.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="extraData">The data bag used to store extra data about this client</param>
        public void RegisterFacebookClient(string appId, string appSecret, string displayName, IDictionary<string, object> extraData)
        {
            RegisterClient(new FacebookClient(appId, appSecret), displayName, extraData);
        }

        /// <summary>
        /// Registers the Microsoft account client.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        public void RegisterMicrosoftClient(string clientId, string clientSecret)
        {
            RegisterMicrosoftClient(clientId, clientSecret, displayName: "Microsoft");
        }

        /// <summary>
        /// Registers the Microsoft account client.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="displayName">The display name.</param>
        public void RegisterMicrosoftClient(string clientId, string clientSecret, string displayName)
        {
            RegisterMicrosoftClient(clientId, clientSecret, displayName, new Dictionary<string, object>());
        }

        /// <summary>
        /// Registers the Microsoft account client.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="extraData">The data bag used to store extra data about this client</param>
        public void RegisterMicrosoftClient(string clientId, string clientSecret, string displayName, IDictionary<string, object> extraData)
        {
            RegisterClient(new MicrosoftClient(clientId, clientSecret), displayName, extraData);
        }

        /// <summary>
        /// Registers the Twitter client.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        public void RegisterTwitterClient(string consumerKey, string consumerSecret)
        {
            RegisterTwitterClient(consumerKey, consumerSecret, displayName: "Twitter");
        }

        /// <summary>
        /// Registers the Twitter client.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        /// <param name="displayName">The display name.</param>
        public void RegisterTwitterClient(string consumerKey, string consumerSecret, string displayName)
        {
            RegisterTwitterClient(consumerKey, consumerSecret, displayName, new Dictionary<string, object>());
        }

        /// <summary>
        /// Registers the Twitter client.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="extraData">The data bag used to store extra data about this client</param>
        public void RegisterTwitterClient(string consumerKey, string consumerSecret, string displayName, IDictionary<string, object> extraData)
        {
            var twitterClient = new TwitterClient(consumerKey, consumerSecret);
            RegisterClient(twitterClient, displayName, extraData);
        }

        /// <summary>
        /// Registers the LinkedIn client.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        public void RegisterLinkedInClient(string consumerKey, string consumerSecret)
        {
            RegisterLinkedInClient(consumerKey, consumerSecret, displayName: "LinkedIn");
        }

        /// <summary>
        /// Registers the LinkedIn client.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        /// <param name="displayName">The display name.</param>
        public void RegisterLinkedInClient(string consumerKey, string consumerSecret, string displayName)
        {
            RegisterLinkedInClient(consumerKey, consumerSecret, displayName, new Dictionary<string, object>());
        }

        /// <summary>
        /// Registers the LinkedIn client.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="extraData">The data bag used to store extra data about this client</param>
        public void RegisterLinkedInClient(string consumerKey, string consumerSecret, string displayName, IDictionary<string, object> extraData)
        {
            var linkedInClient = new LinkedInClient(consumerKey, consumerSecret);
            RegisterClient(linkedInClient, displayName, extraData);
        }

        /// <summary>
        /// Registers the Google client.
        /// </summary>
        public void RegisterGoogleClient()
        {
            RegisterGoogleClient(displayName: "Google");
        }

        /// <summary>
        /// Registers the Google client.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        public void RegisterGoogleClient(string displayName)
        {
            RegisterClient(new GoogleOpenIdClient(), displayName, new Dictionary<string, object>());
        }

        /// <summary>
        /// Registers the Google client.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="extraData">The data bag.</param>
        public void RegisterGoogleClient(string displayName, IDictionary<string, object> extraData)
        {
            RegisterClient(new GoogleOpenIdClient(), displayName, extraData);
        }

        /// <summary>
        /// Registers the Yahoo client.
        /// </summary>
        public void RegisterYahooClient()
        {
            RegisterYahooClient(displayName: "Yahoo");
        }

        /// <summary>
        /// Registers the Yahoo client.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        public void RegisterYahooClient(string displayName)
        {
            RegisterYahooClient(displayName, new Dictionary<string, object>());
        }

        /// <summary>
        /// Registers the Yahoo client.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="extraData">The data bag.</param>
        public void RegisterYahooClient(string displayName, IDictionary<string, object> extraData)
        {
            RegisterClient(new YahooOpenIdClient(), displayName, extraData);
        }

        /// <summary>
        /// Registers an authentication client.
        /// </summary>
        /// <param name="client">The client to be registered.</param>
        [CLSCompliant(false)]
        public void RegisterClient(IAuthenticationClient client)
        {
            RegisterClient(client, displayName: null, extraData: new Dictionary<string, object>());
        }

        /// <summary>
        /// Registers an authentication client.
        /// </summary>
        /// <param name="client">The client to be registered</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="extraData">The data bag used to store extra data about the specified client</param>
        [CLSCompliant(false)]
        public void RegisterClient(IAuthenticationClient client, string displayName, IDictionary<string, object> extraData)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            if (String.IsNullOrEmpty(client.ProviderName))
            {
                throw new ArgumentException("InvalidServiceProviderName", "client");
            }

            if (_authenticationClients.ContainsKey(client.ProviderName))
            {
                throw new ArgumentException("ServiceProviderNameExists", "client");
            }

            var clientData = new AuthenticationClientData(client, displayName, extraData);
            _authenticationClients.Add(client.ProviderName, clientData);
        }

        #endregion

        #region GetOAuthClientData


        /// <summary>
        /// Gets the OAuth client data of the specified provider name.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>The AuthenticationClientData of the specified provider name.</returns>
        public AuthenticationClientData GetOAuthClientData(string providerName)
        {
            if (providerName == null)
            {
                throw new ArgumentNullException("providerName");
            }

            return _authenticationClients[providerName];
        }

        /// <summary>
        /// Tries getting the OAuth client data of the specified provider name.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="clientData">The client data of the specified provider name.</param>
        /// <returns><c>true</c> if the client data is found for the specified provider name. Otherwise, <c>false</c></returns>
        public bool TryGetOAuthClientData(string providerName, out AuthenticationClientData clientData)
        {
            if (providerName == null)
            {
                throw new ArgumentNullException("providerName");
            }

            return _authenticationClients.TryGetValue(providerName, out clientData);
        }

        public virtual IAuthenticationClient GetOAuthClient(string providerName)
        {
            if (!_authenticationClients.ContainsKey(providerName))
            {
                throw new ArgumentException("ServiceProviderNotFound", "providerName");
            }

            return _authenticationClients[providerName].AuthenticationClient;
        }

        public bool TryGetOAuthClient(string provider, out IAuthenticationClient client)
        {
            if (_authenticationClients.ContainsKey(provider))
            {
                client = _authenticationClients[provider].AuthenticationClient;
                return true;
            }
            else
            {
                client = null;
                return false;
            }
        }

        #endregion
    }
}
