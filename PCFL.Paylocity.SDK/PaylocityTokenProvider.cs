using Microsoft.Identity.Client;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCFL.Paylocity.SDK
{
    public class PaylocityTokenProvider : ITokenProvider
    {
        private readonly string _baseAddress = @"https://api.paylocity.com/IdentityServer/connect/token";
        private readonly string _grantType = "client_credentials";
        private readonly string _clientID;
        private readonly string _clientSecret;
        private HttpClient _client;
        public PaylocityTokenProvider(string clientID, string clientSecret)
        {
            _clientID = clientID;
            _clientSecret = clientSecret;
            _client = new HttpClient();
            var authenticationString = $"{_clientID}:{_clientSecret}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(authenticationString));
            _client.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            var form = new Dictionary<string, string>
                {
                    {"grant_type", _grantType},
                    {"scope", "WebLinkAPI"},
                };
            HttpResponseMessage tokenResponse = await _client.PostAsync(_baseAddress, new FormUrlEncodedContent(form));
            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            Token tok = JsonConvert.DeserializeObject<Token>(jsonContent);
            return new AuthenticationHeaderValue("Bearer", tok.AccessToken);
        }
    }
}
