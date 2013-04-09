using System;
using System.Collections.Generic;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;

namespace ToileDeFond.Security
{
    public interface IOAuthRegisteredClientCollection
    {
        /// <summary>
        /// Gets the collection of all registered authentication client;
        /// </summary>
        /// <returns></returns>
        ICollection<AuthenticationClientData> OAuthRegisteredClientData { get; }

        /// <summary>
        /// Registers the Facebook client.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="appSecret">The app secret.</param>
        void RegisterFacebookClient(string appId, string appSecret);

        /// <summary>
        /// Registers the Facebook client.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="appSecret">The app secret.</param>
        /// <param name="displayName">The display name of the client.</param>
        void RegisterFacebookClient(string appId, string appSecret, string displayName);

        /// <summary>
        /// Registers the Facebook client.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="appSecret">The app secret.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="extraData">The data bag used to store extra data about this client</param>
        void RegisterFacebookClient(string appId, string appSecret, string displayName, IDictionary<string, object> extraData);

        /// <summary>
        /// Registers the Microsoft account client.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        void RegisterMicrosoftClient(string clientId, string clientSecret);

        /// <summary>
        /// Registers the Microsoft account client.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="displayName">The display name.</param>
        void RegisterMicrosoftClient(string clientId, string clientSecret, string displayName);

        /// <summary>
        /// Registers the Microsoft account client.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="extraData">The data bag used to store extra data about this client</param>
        void RegisterMicrosoftClient(string clientId, string clientSecret, string displayName, IDictionary<string, object> extraData);

        /// <summary>
        /// Registers the Twitter client.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        void RegisterTwitterClient(string consumerKey, string consumerSecret);

        /// <summary>
        /// Registers the Twitter client.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        /// <param name="displayName">The display name.</param>
        void RegisterTwitterClient(string consumerKey, string consumerSecret, string displayName);

        /// <summary>
        /// Registers the Twitter client.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="extraData">The data bag used to store extra data about this client</param>
        void RegisterTwitterClient(string consumerKey, string consumerSecret, string displayName, IDictionary<string, object> extraData);

        /// <summary>
        /// Registers the LinkedIn client.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        void RegisterLinkedInClient(string consumerKey, string consumerSecret);

        /// <summary>
        /// Registers the LinkedIn client.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        /// <param name="displayName">The display name.</param>
        void RegisterLinkedInClient(string consumerKey, string consumerSecret, string displayName);

        /// <summary>
        /// Registers the LinkedIn client.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="extraData">The data bag used to store extra data about this client</param>
        void RegisterLinkedInClient(string consumerKey, string consumerSecret, string displayName, IDictionary<string, object> extraData);

        /// <summary>
        /// Registers the Google client.
        /// </summary>
        void RegisterGoogleClient();

        /// <summary>
        /// Registers the Google client.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        void RegisterGoogleClient(string displayName);

        /// <summary>
        /// Registers the Google client.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="extraData">The data bag.</param>
        void RegisterGoogleClient(string displayName, IDictionary<string, object> extraData);

        /// <summary>
        /// Registers the Yahoo client.
        /// </summary>
        void RegisterYahooClient();

        /// <summary>
        /// Registers the Yahoo client.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        void RegisterYahooClient(string displayName);

        /// <summary>
        /// Registers the Yahoo client.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="extraData">The data bag.</param>
        void RegisterYahooClient(string displayName, IDictionary<string, object> extraData);

        /// <summary>
        /// Registers an authentication client.
        /// </summary>
        /// <param name="client">The client to be registered.</param>
        [CLSCompliant(false)]
        void RegisterClient(IAuthenticationClient client);

        /// <summary>
        /// Registers an authentication client.
        /// </summary>
        /// <param name="client">The client to be registered</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="extraData">The data bag used to store extra data about the specified client</param>
        [CLSCompliant(false)]
        void RegisterClient(IAuthenticationClient client, string displayName, IDictionary<string, object> extraData);

        /// <summary>
        /// Tries getting the OAuth client data of the specified provider name.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="clientData">The client data of the specified provider name.</param>
        /// <returns><c>true</c> if the client data is found for the specified provider name. Otherwise, <c>false</c></returns>
        bool TryGetOAuthClientData(string providerName, out AuthenticationClientData clientData);

        IAuthenticationClient GetOAuthClient(string providerName);

        AuthenticationClientData GetOAuthClientData(string providerName);

        bool TryGetOAuthClient(string provider, out IAuthenticationClient client);
    }
}