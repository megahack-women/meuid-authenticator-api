using Newtonsoft.Json;
using System;

namespace Authenticator.Models
{
    public class TokenModel
    {
        /// <summary>
        /// Token de autorização que deverá ser utilizado para obter os dados do cliente
        /// </summary>
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Token utilizado para obter o token de acesso
        /// </summary>
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}
