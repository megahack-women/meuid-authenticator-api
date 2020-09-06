using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Authenticator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

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
        public async Task<ActionResult<IdentityModel>> GetAsync(string token)
        {
            IdentityModel identityModel = null;

            if (!string.IsNullOrEmpty(token))
            {
                using (var httpClient = new HttpClient())
                {
                    var refreshTokenObject = new
                    {
                        refresh_token = token,
                        grant_type = "refresh_token",
                        client_id = _config.GetValue<string>("Credentials:ClientId"),
                        client_secret = _config.GetValue<string>("Credentials:ClientSecret"),
                    };

                    var refreshTokenJson = new StringContent(
                        JsonConvert.SerializeObject(refreshTokenObject), Encoding.UTF8, "application/json");

                    var tokenModel = new TokenModel();

                    using (var response = await httpClient.PostAsync("https://api-v3.idwall.co/token", refreshTokenJson))
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        tokenModel = JsonConvert.DeserializeObject<TokenModel>(responseBody);
                    }

                    identityModel.Token = tokenModel.RefreshToken;

                    using (var response = await httpClient.GetAsync("https://api-v3.idwall.co/meuid/data"))
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        identityModel = JsonConvert.DeserializeObject<IdentityModel>(responseBody);
                    }
                }
            }

            return identityModel;
        }

        // POST authentication
        [HttpPost]
        public async Task<ActionResult<IdentityModel>> PostAsync([FromBody] AuthorizationModel authorizationModel)
        {
            var credentials = new
            {
                ClientId = _config.GetValue<string>("Credentials:ClientId"),
                ClientSecret = _config.GetValue<string>("Credentials:ClientSecret")
            };

            using (var httpClient = new HttpClient())
            {
                var authorizationObject = new 
                {
                    authorization_code = authorizationModel.Code,
                    code_verifier = authorizationModel.CodeVerifier,
                    client_id = credentials.ClientId,
                    client_secret = credentials.ClientSecret,
                    grant_type = "authorization_code"
                };

                var authorizationJson = new StringContent(
                    JsonConvert.SerializeObject(authorizationObject), Encoding.UTF8, "application/json");

                var tokenModel = new TokenModel();

                using (var response = await httpClient.PostAsync("https://api-v3.idwall.co/token", authorizationJson))
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    tokenModel = JsonConvert.DeserializeObject<TokenModel>(responseBody);
                }

                var identityModel = new IdentityModel
                {
                    Token = tokenModel.RefreshToken
                };

                if (!string.IsNullOrEmpty(tokenModel.AccessToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Authorization", tokenModel.AccessToken);

                    using (var response = await httpClient.GetAsync("https://api-v3.idwall.co/meuid/data"))
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        identityModel = JsonConvert.DeserializeObject<IdentityModel>(responseBody);
                    }
                }

                return identityModel;
            }
        }
    }
}
