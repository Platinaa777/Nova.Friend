using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Nova.Friend.Infrastructure.OutboxPattern;

public class OutboxMessage
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Id { get; set; } = string.Empty;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Key { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime StartedAtUtc { get; set; }
    public DateTime? HandledAtUtc { get; set; }
    public string? Error { get; set; } = string.Empty;
}