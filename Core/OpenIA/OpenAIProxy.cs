namespace WibboEmulator.Core.OpenIA;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public static class OpenAIProxy
{
    private const string BASE_URL = "https://api.openai.com/v1/";
    private static readonly HttpClient OpenAIClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };
    private static DateTime _lastRequestChat = DateTime.Now;
    private static DateTime _lastRequestAudio = DateTime.Now;
    private static bool _waitedChatAPI;
    private static bool _waitedAudioAPI;

    public static void Initialize(string apiKey) => OpenAIClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

    public static async Task<byte[]> TextToSpeech(string nameVoice, string text)
    {
        if (!IsReadyToSendAudio)
        {
            return null;
        }

        try
        {
            var request = new
            {
                model = "tts-1",
                input = text,
                voice = nameVoice
            };
            var requestJson = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
            var httpResponseMessage = await OpenAIClient.PostAsync(BASE_URL + "audio/speech", requestContent);

            _lastRequestAudio = DateTime.Now;
            _waitedAudioAPI = false;

            if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var audioBytes = await httpResponseMessage.Content.ReadAsByteArrayAsync();

            return audioBytes;
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex.ToString());
        }

        _waitedAudioAPI = false;
        return null;
    }

    public static async Task<ChatCompletionMessage> SendChatMessage(List<ChatCompletionMessage> messagesSend, bool audioMode = false, string audioVoice = "alloy")
    {
        try
        {
            if (!IsReadyToSendChat)
            {
                return null;
            }

            _waitedChatAPI = true;

            var request = new
            {
                messages = messagesSend.ToArray(),
                model = audioMode ? "gpt-4o-mini-audio-preview" : "gpt-4o-mini",
                max_completion_tokens = audioMode ? (int?)null : 150,
                temperature = audioMode ? (double?)null : 0.6,
                stop = audioMode ? null : "\n",
                modalities = audioMode ? new string[] { "text", "audio" } : null,
                audio = audioMode ? new { voice = audioVoice, format = "wav" } : null
            };


            var requestJson = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var requestContent = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
            var httpResponseMessage = await OpenAIClient.PostAsync(BASE_URL + "chat/completions", requestContent);

            _lastRequestChat = DateTime.Now;
            _waitedChatAPI = false;

            if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            {
                var errorContent = await httpResponseMessage.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {errorContent}");
                return null;
            }

            var jsonString = await httpResponseMessage.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeAnonymousType(jsonString, new
            {
                choices = new[] { new { message = new ChatCompletionMessage { Role = string.Empty, Content = string.Empty, Audio = new() } } },
                error = new { message = string.Empty }
            });

            if (responseObject == null || !string.IsNullOrEmpty(responseObject?.error?.message)) // Check for errors
            {
                return null;
            }

            var messagesGtp = responseObject?.choices[0]?.message;
            return messagesGtp;
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex.ToString());
            _waitedChatAPI = false;
        }

        return null;
    }

    public static bool IsReadyToSendChat
    {
        get
        {
            var timespan = DateTime.Now - _lastRequestChat;
            return timespan.TotalSeconds > 3 && !_waitedChatAPI;
        }
    }

    public static bool IsReadyToSendAudio
    {
        get
        {
            var timespan = DateTime.Now - _lastRequestAudio;
            return timespan.TotalSeconds > 3 && !_waitedAudioAPI;
        }
    }
}
