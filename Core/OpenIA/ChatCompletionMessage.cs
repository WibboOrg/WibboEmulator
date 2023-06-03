namespace WibboEmulator.Core.OpenIA;

using Newtonsoft.Json;

public class ChatCompletionMessage
{
    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = "ChatGPT";
}
