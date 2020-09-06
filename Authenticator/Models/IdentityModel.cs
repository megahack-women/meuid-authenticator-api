using Newtonsoft.Json;

namespace Authenticator.Models
{
    public class IdentityModel
    {
        [JsonProperty(PropertyName = "data")]
        public Data Data { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }

    public class Personal
    {
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }
    }

    public class Document
    {
        [JsonProperty(PropertyName = "name")]
        public string Nome { get; set; }

        [JsonProperty(PropertyName = "numeroCpf")]
        public string NumeroCpf { get; set; }

        [JsonProperty(PropertyName = "numeroRg")]
        public string NumeroRg { get; set; }

        [JsonProperty(PropertyName = "birthDate")]
        public string DataNascimento { get; set; }
    }

    public class Data
    {
        [JsonProperty(PropertyName = "personal")]
        public Personal Personal { get; set; }

        [JsonProperty(PropertyName = "document")]
        public Document Document { get; set; }
    }
}
