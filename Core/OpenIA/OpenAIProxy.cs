namespace WibboEmulator.Core.OpenIA;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class OpenAIProxy : IDisposable
{
    private const string BASE_URL = "https://api.openai.com/v1/";
    private readonly HttpClient _openAIClient;
    private DateTime _lastRequestChat;
    private DateTime _lastRequestAudio;
    private bool _waitedChatAPI;
    private bool _waitedAudioAPI;

    public OpenAIProxy(string apiKey)
    {
        this._openAIClient = new()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        this._openAIClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        this._lastRequestChat = DateTime.Now;
        this._lastRequestAudio = DateTime.Now;
    }

    public async Task<byte[]> TextToSpeech(string nameVoice, string text)
    {
        if (this.IsReadyToSendAudio() == false)
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
            var httpResponseMessage = await this._openAIClient.PostAsync(BASE_URL + "audio/speech", requestContent);

            this._lastRequestAudio = DateTime.Now;
            this._waitedAudioAPI = false;

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

        this._waitedAudioAPI = false;
        return null;
    }

    public async Task<ChatCompletionMessage> SendChatMessage(List<ChatCompletionMessage> messagesSend)
    {
        try
        {
            if (this.IsReadyToSendChat() == false)
            {
                return null;
            }

            this._waitedChatAPI = true;

            var request = new
            {
                messages = messagesSend.ToArray(),
                model = "gpt-3.5-turbo-1106",
                max_tokens = 150,
                temperature = 0.6,
                stop = "\n"
            };

            var requestJson = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
            var httpResponseMessage = await this._openAIClient.PostAsync(BASE_URL + "chat/completions", requestContent);

            this._lastRequestChat = DateTime.Now;
            this._waitedChatAPI = false;

            if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var jsonString = await httpResponseMessage.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeAnonymousType(jsonString, new
            {
                choices = new[] { new { message = new ChatCompletionMessage { Role = string.Empty, Content = string.Empty } } },
                error = new { message = string.Empty }
            });

            if (responseObject == null || !string.IsNullOrEmpty(responseObject?.error?.message))  // Check for errors
            {
                return null;
            }

            var messagesGtp = responseObject?.choices[0]?.message;

            if (messagesGtp == null)
            {
                return null;
            }

            return messagesGtp;
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex.ToString());
            this._waitedChatAPI = false;
        }

        return null;
    }

    public bool IsReadyToSendChat()
    {
        var timespan = DateTime.Now - this._lastRequestChat;
        return timespan.TotalSeconds > 3 && !this._waitedChatAPI;
    }

    public bool IsReadyToSendAudio()
    {
        var timespan = DateTime.Now - this._lastRequestAudio;
        return timespan.TotalSeconds > 3 && !this._waitedAudioAPI;
    }

    public void Dispose()
    {
        this._openAIClient.Dispose();

        GC.SuppressFinalize(this);
    }
}
