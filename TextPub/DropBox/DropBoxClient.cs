using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Elmah;
using OAuth;
using TextPub.DropBox.Models;

namespace TextPub.DropBox
{
    /// <summary>
    /// Handles all communication with the DropBox API.
    /// </summary>
    public class DropBoxClient : OAuthBase
    {
        private string _consumerToken;
        private string _consumerSecret;
        private string _userToken;
        private string _userSecret;

        private JavaScriptSerializer _jsonSerializer = new JavaScriptSerializer();

        private const string _root = "sandbox";
        private const string _version = "1";
        private const string _apiBaseUrl = "https://api.dropbox.com";
        private const string _apiContentBaseUrl = "https://api-content.dropbox.com";
        private const string _wwwBaseUrl = "https://www.dropbox.com";

        public DropBoxClient(string consumerToken, string consumerSecret)
        {
            _consumerToken = consumerToken;
            _consumerSecret = consumerSecret;

            _jsonSerializer.RegisterConverters(new JavaScriptConverter[] { new DeltaJavaScriptConverter() });
        }

        public DropBoxClient(string consumerToken, string consumerSecret, string userToken, string userSecret)
            : this(consumerToken, consumerSecret)
        {
            _userToken = userToken;
            _userSecret = userSecret;
        }

        public DropBoxClient(string consumerToken, string consumerSecret, UserCredentials userCredentials)
            : this(consumerToken, consumerSecret)
        {
            if (userCredentials != null)
            {
                _userSecret = userCredentials.Secret;
                _userToken = userCredentials.Token;
            }
        }
     
        public void SetUserCredentials(string userToken, string userSecret)
        {
            _userSecret = userSecret;
            _userToken = userToken;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cursor"></param>
        /// <returns></returns>
        internal Delta GetDelta(string cursor)
        {
            string parameters = null;
            string requestUri = string.Format("{0}/{1}/delta", _apiBaseUrl, _version);
            
            if (!string.IsNullOrWhiteSpace(cursor))
            {
                parameters = string.Format("cursor={0}", UrlEncode(cursor));
            }

            HttpWebRequest request = CreateOAuthRequest(requestUri, parameters, WebRequestMethods.Http.Post);
            try
            {
                var response = request.GetResponse() as HttpWebResponse;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    // TODO: Log
                    throw new Exception("TODO");
                }

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string json = reader.ReadToEnd();

                    return _jsonSerializer.Deserialize<Delta>(json);
                }
            }
            catch (WebException e)
            {
                LogError(e);
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal byte[] GetFile(string path)
        {
            var requestUri = string.Format("{0}/{1}/files/{2}/{3}", _apiContentBaseUrl, _version, _root, EncodePath(path));

            HttpWebRequest request = CreateOAuthRequest(requestUri);

            try
            {
                var response = request.GetResponse() as HttpWebResponse;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    // TODO: Log
                    throw new Exception("TODO");
                }

                using (Stream responseStream = response.GetResponseStream())
                {

                    const int readSize = 256;
                    byte[] buffer = new byte[readSize];
                    using (MemoryStream ms = new MemoryStream())
                    {

                        int count = responseStream.Read(buffer, 0, readSize);
                        while (count > 0)
                        {
                            ms.Write(buffer, 0, count);
                            count = responseStream.Read(buffer, 0, readSize);
                        }
                        ms.Position = 0;
                        responseStream.Close();

                        return ms.ToArray();
                    }
                }
            }
            catch (WebException e)
            {
                LogError(e);
            }
            return new byte[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public UserCredentials GetRequestToken()
        {
            var requestUri = string.Format("{0}/{1}/oauth/request_token", _apiBaseUrl, _version);

            var request = CreateOAuthRequest(requestUri);

            try
            {
                var response = request.GetResponse() as HttpWebResponse;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    // TODO: Log
                    throw new Exception("TODO");
                }

                using (var reader = new StreamReader(response.GetResponseStream()))
                {

                    NameValueCollection responseParams = HttpUtility.ParseQueryString(reader.ReadToEnd());

                    return new UserCredentials
                    {
                        Token = responseParams["oauth_token"],
                        Secret = responseParams["oauth_token_secret"]
                    };
                }
            }
            catch (WebException e)
            {
                LogError(e);
            }
            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public string GetAuthorizationUrl(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return String.Format("{0}/{1}/oauth/authorize?oauth_token={2}", _wwwBaseUrl, _version, _userToken);
            }
            return String.Format(
                "{0}/{1}/oauth/authorize?oauth_token={2}&amp;oauth_callback={3}", 
                _wwwBaseUrl,
                _version,
                _userToken, 
                HttpUtility.UrlEncode(returnUrl)
            );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public UserCredentials GetAccessToken()
        {
            var requestUri = string.Format("{0}/{1}/oauth/access_token", _apiBaseUrl, _version);

            HttpWebRequest request = CreateOAuthRequest(requestUri);

            try
            {
                var response = request.GetResponse() as HttpWebResponse;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    // TODO: Log
                    throw new Exception("TODO");
                }

                using (var reader = new StreamReader(response.GetResponseStream()))
                {

                    NameValueCollection responseParams = HttpUtility.ParseQueryString(reader.ReadToEnd());

                    return new UserCredentials
                    {
                        Token = responseParams["oauth_token"],
                        Secret = responseParams["oauth_token_secret"]
                    };
                }
            }
            catch (WebException e)
            {
                LogError(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string EncodePath(string path)
        {
            string[] pathParts = path.TrimStart('/').Split('/');
            for (int i = 0; i < pathParts.Length; i++)
            {
                pathParts[i] = UrlEncode(pathParts[i]);
            }
            return string.Join("/", pathParts);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUriString"></param>
        /// <returns></returns>
        private HttpWebRequest CreateOAuthRequest(string requestUriString)
        {
            return CreateOAuthRequest(requestUriString, string.Empty, WebRequestMethods.Http.Get);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUriString"></param>
        /// <param name="parameters"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        private HttpWebRequest CreateOAuthRequest(string requestUriString, string parameters, string httpMethod)
        {
            var requestUri = new Uri(string.Format("{0}?{1}", requestUriString, parameters));
            
            
            string nonce = GenerateNonce();
            string timeStamp = GenerateTimeStamp();

            string normalizedUrl;
            string oAuthParameters;
            
            string signature = GenerateSignature(
                requestUri,
                _consumerToken,
                _consumerSecret,
                _userToken,
                _userSecret,
                httpMethod,
                timeStamp,
                nonce,
                OAuthBase.SignatureTypes.HMACSHA1,
                out normalizedUrl,
                out oAuthParameters
            );

            HttpWebRequest request;

            if (httpMethod == WebRequestMethods.Http.Post)
            {
                requestUri = new Uri(string.Format("{0}?{1}&oauth_signature={2}", requestUriString, oAuthParameters, UrlEncode(signature)));
                request = WebRequest.Create(requestUri) as HttpWebRequest;
                request.Method = httpMethod;
                //SetRequestBody(request, string.IsNullOrWhiteSpace(parameters) ? oAuthParameters : string.Format("{0}&{1}", parameters, oAuthParameters));
            }
            else
            {
                request = WebRequest.Create(requestUri) as HttpWebRequest;
                request.Method = httpMethod;

                SetRequestAuthorizationHeader(request, nonce, timeStamp, signature);
            }

            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="nonce"></param>
        /// <param name="timeStamp"></param>
        /// <param name="signature"></param>
        private void SetRequestAuthorizationHeader(HttpWebRequest request, string nonce, string timeStamp, string signature)
        {
            var authParams = new NameValueCollection();
            authParams.Add("realm", _apiBaseUrl);
            authParams.Add(OAuthConsumerKeyKey, _consumerToken);
            authParams.Add(OAuthNonceKey, nonce);
            authParams.Add(OAuthTimestampKey, timeStamp);
            authParams.Add(OAuthSignatureMethodKey, HMACSHA1SignatureType);
            authParams.Add(OAuthVersionKey, OAuthVersion);
            authParams.Add(OAuthSignatureKey, signature);

            if (_userToken != null)
            {
                authParams.Add(OAuthTokenKey, _userToken);
            }

            request.Headers.Add(
                "Authorization",
                string.Format(
                    @" OAuth {0}",
                    string.Join(
                        ", ",
                        Array.ConvertAll(
                            authParams.AllKeys,
                            key => string.Format("{0}=\"{1}\"", key, UrlEncode(authParams[key]))
                        )
                    )
                )
            );
        }

        private void LogError(WebException e)
        {
            var reader = new StreamReader(e.Response.GetResponseStream());
            string rawJson = reader.ReadToEnd();
            //dynamic json = _jsonSerializer.DeserializeObject(rawJson);

            Elmah.ErrorLog.GetDefault(null).Log(new Error(new Exception(rawJson, e)));
        }
    }
}

