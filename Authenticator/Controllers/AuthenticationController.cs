using System;
using System.Collections.Generic;
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
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthenticationController(IConfiguration config)
        {
            _config = config;
        }

        // GET api/authentication
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { DateTime.Now.ToLongDateString() };
        }

        // POST api/authentication
        [HttpPost]
        public async Task PostAsync([FromBody] AuthorizationModel authorizationModel)
        {
            var credentials = new
            {
                ClientId = _config.GetValue<string>(
                "Credentials:ClientId"),
                ClientSecret = _config.GetValue<string>(
                "Credentials:ClientSecret")
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

                var accessToken = string.Empty;

                using (var response = await httpClient.PostAsync("https://api-v3.idwall.co/token", authorizationJson))
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    var tokenModel = JsonConvert.DeserializeObject<TokenModel>(responseBody);

                    accessToken = tokenModel.AccessToken;
                }

                httpClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Authorization", accessToken);

                using (var response = await httpClient.GetAsync("https://api-v3.idwall.co/meuid/data"))
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    var identityModel = JsonConvert.DeserializeObject<IdentityModel>(responseBody);
                }
            }
        }
    }
}
