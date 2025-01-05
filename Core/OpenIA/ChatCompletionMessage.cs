namespace WibboEmulator.Core.OpenIA;

using Newtonsoft.Json;

public class ChatCompletionMessage
{
    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("audio")]
    public AudioData Audio { get; set; }
}

public class AudioData
{
    [JsonProperty("data")]
    public string Data { get; set; }

    [JsonProperty("transcript")]
    public string Transcript { get; set; }
}
