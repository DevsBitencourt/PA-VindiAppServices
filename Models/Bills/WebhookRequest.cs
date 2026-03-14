using System.Text.Json.Serialization;

namespace Vindi.Webhook.Models.Bills
{
    public class WebhookRequest
    {
        [JsonPropertyName("event")]
        public Event? @event { get; set; }
    }
}
