
using System.Text.Json.Serialization;

namespace Authenticator.Models
{
    public class IdentityModel
    {
        [JsonPropertyName("auth")]
        public AuthorizationModel Auth { get; set; }

        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }

    public class Personal
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }
    }

    public class Document
    {
        [JsonPropertyName("name")]
        public string Nome { get; set; }

        [JsonPropertyName("numeroCpf")]
        public string NumeroCpf { get; set; }

        [JsonPropertyName("numeroRg")]
        public string NumeroRg { get; set; }

        [JsonPropertyName("birthDate")]
        public string DataNascimento { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("personal")]
        public Personal Personal { get; set; }

        [JsonPropertyName("document")]
        public Document Document { get; set; }
    }
}
