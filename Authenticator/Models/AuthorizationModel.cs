using System.Text.Json.Serialization;

namespace Authenticator.Models
{
    public class AuthorizationModel
    {
        /// <summary>
        /// Código do processo de autorização do usuário MeuID. 
        /// Servirá de insumo para a obtenção dos dados pessoais.
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// Código verificador do processo de autorização. 
        /// Servirá de insumo para obtenção dos dados pessoais.
        /// </summary>
        [JsonPropertyName("code_verifier")]
        public string CodeVerifier { get; set; }
    }
}
