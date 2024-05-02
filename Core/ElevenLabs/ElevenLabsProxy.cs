namespace WibboEmulator.Core.ElevenLabs;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public static class ElevenLabsProxy
{
    private const string BASE_URL = "https://api.elevenlabs.io/v1/";
    private static readonly HttpClient ApiClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };
    private static DateTime _lastRequestAudio = DateTime.Now;
    private static bool _waitedAudioAPI;

    public static void Initialize(string apiKey) => ApiClient.DefaultRequestHeaders.Add("xi-api-key", apiKey);

    public static async Task<byte[]> TextToSpeech(string modelId, string text)
    {
        if (!IsReadyToSendAudio)
        {
            return null;
        }

        try
        {
            var request = new
            {
                model_id = "eleven_multilingual_v1",
                text
            };
            var requestJson = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
            var httpResponseMessage = await ApiClient.PostAsync(BASE_URL + "text-to-speech/" + modelId, requestContent);

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

    public static bool IsReadyToSendAudio
    {
        get
        {
            var timespan = DateTime.Now - _lastRequestAudio;
            return timespan.TotalSeconds > 3 && !_waitedAudioAPI;
        }
    }
}
