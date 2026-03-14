using System.Text.Json.Serialization;

namespace Vindi.Webhook.Models.Bills
{
    public class Event
    {
        [JsonPropertyName("type")]
        public string? type { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? created_at { get; set; }

        [JsonPropertyName("data")]
        public object? data { get; set; }
    }
}
