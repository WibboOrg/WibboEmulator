namespace WibboEmulator.Core.ElevenLabs;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ElevenLabsProxy : IDisposable
{
    private const string BASE_URL = "https://api.elevenlabs.io/v1/";
    private readonly HttpClient _apiClient;
    private DateTime _lastRequestAudio;
    private bool _waitedAudioAPI;

    public ElevenLabsProxy(string apiKey)
    {
        this._apiClient = new()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        this._apiClient.DefaultRequestHeaders.Add("xi-api-key", apiKey);

        this._lastRequestAudio = DateTime.Now;
    }

    public async Task<byte[]> TextToSpeech(string modelId, string text)
    {
        if (this.IsReadyToSendAudio() == false)
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
            var httpResponseMessage = await this._apiClient.PostAsync(BASE_URL + "text-to-speech/" + modelId, requestContent);

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
    public bool IsReadyToSendAudio()
    {
        var timespan = DateTime.Now - this._lastRequestAudio;
        return timespan.TotalSeconds > 3 && !this._waitedAudioAPI;
    }

    public void Dispose()
    {
        this._apiClient.Dispose();

        GC.SuppressFinalize(this);
    }
}
