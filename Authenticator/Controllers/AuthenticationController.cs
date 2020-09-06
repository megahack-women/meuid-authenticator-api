using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Authenticator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System;

namespace Authenticator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthenticationController(IConfiguration config)
        {
            _config = config;
        }

        // GET authentication
        [HttpGet]
        public async Task<ActionResult<InviteModel>> GetAsync()
        {
            var invite = new InviteModel();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization
                                     = new AuthenticationHeaderValue( 
                                        _config.GetValue<string>("MeuIDCredentials:ApiToken"));

                var opportunity = new
                {
                    opportunityId = _config.GetValue<string>("MeuIDCredentials:OpportunityId")
                };

                var opportunityContent = new StringContent(
                    JsonSerializer.Serialize(opportunity), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(
                    "https://api-v3.idwall.co/meuid/invites/opportunity", opportunityContent))
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    invite = JsonSerializer.Deserialize<InviteModel>(responseBody);
                }
            }

            return invite;
        }

        // POST authentication
        [HttpPost]
        public async Task<ActionResult<IdentityModel>> PostAsync([FromBody] AuthorizationModel authorizationModel)
        {
            var identityModel = await GetAuthorizedData(authorizationModel);

            return identityModel;
        }

        private async Task<IdentityModel> GetAuthorizedData(AuthorizationModel authorizationModel)
        {
            var identityModel = new IdentityModel
            {
                Auth = authorizationModel
            };

            using (var httpClient = new HttpClient())
            {
                var authorization = new
                {
                    authorization_code = authorizationModel.Code,
                    code_verifier = authorizationModel.CodeVerifier,
                    client_id = _config.GetValue<string>("MeuIDCredentials:ClientId"),
                    client_secret = _config.GetValue<string>("MeuIDCredentials:ClientSecret"),
                    grant_type = "authorization_code"
                };

                var authorizationContent = new StringContent(
                    JsonSerializer.Serialize(authorization), Encoding.UTF8, "application/json");

                var token = await GetAccessToken(httpClient, authorizationContent);

                if (!string.IsNullOrEmpty(token.AccessToken))
                {
                    identityModel = await GetUserData(identityModel, httpClient, token);
                }
            }

            return identityModel;
        }

        private static async Task<TokenModel> GetAccessToken(HttpClient httpClient, StringContent authorizationJson)
        {
            var tokenModel = new TokenModel();
            using (var response = await httpClient.PostAsync("https://api-v3.idwall.co/token", authorizationJson))
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                tokenModel = JsonSerializer.Deserialize<TokenModel>(responseBody);
            }

            return tokenModel;
        }

        private static async Task<IdentityModel> GetUserData(IdentityModel identityModel, HttpClient httpClient, TokenModel token)
        {
            httpClient.DefaultRequestHeaders.Authorization
                                     = new AuthenticationHeaderValue(token.AccessToken);

            using (var response = await httpClient.GetAsync("https://api-v3.idwall.co/meuid/data"))
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                identityModel = JsonSerializer.Deserialize<IdentityModel>(responseBody);
            }

            return identityModel;
        }
    }
}
