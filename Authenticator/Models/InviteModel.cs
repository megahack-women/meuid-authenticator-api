using System.Text.Json.Serialization;

namespace Authenticator.Models
{
    public class InviteModel
    {
        [JsonPropertyName("data")]
        public InviteData Data { get; set; }
    }

    public class InviteData
    {
        [JsonPropertyName("inviteUrl")]
        public string InviteUrl { get; set; }
    }
}
