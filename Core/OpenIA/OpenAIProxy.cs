namespace WibboEmulator.Core.OpenIA;

using System.Net.Http;
using Newtonsoft.Json;

public class OpenAIProxy : IDisposable
{
    private readonly HttpClient _openAIClient;
    private readonly Dictionary<int, List<ChatCompletionMessage>> _userMessages;
    private ChatCompletionMessage _firstPrompt;

    public OpenAIProxy(string apiKey)
    {
        this._openAIClient = new();
        this._openAIClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        this._userMessages = new();
    }

    public void Init()
    {
        this._userMessages.Clear();

        var preMsg = new ChatCompletionMessage()
        {
            Content = WibboEnvironment.GetSettings().GetData<string>("openia.prompt"),
            Role = "system",
            Name = "ChatGPT"
        };

        this._firstPrompt = preMsg;
    }

    public void RemoveUserMessages(int userId) => _ = this._userMessages.Remove(userId);

    private void StackMessages(int userId, params ChatCompletionMessage[] message)
    {
        if (this._userMessages.ContainsKey(userId))
        {
            this._userMessages[userId].AddRange(message);
        }
        else
        {
            this._userMessages.Add(userId, new List<ChatCompletionMessage>(message));
        }

        this._userMessages[userId] = this._userMessages[userId].TakeLast(30).ToList();
    }

    //Public method to Send messages to OpenAI
    public Task<ChatCompletionMessage[]> SendChatMessage(int userId, string name, string message)
    {
        var chatMsg = new ChatCompletionMessage()
        {
            Content = message,
            Role = "user",
            Name = name
        };

        return this.SendChatMessage(userId, chatMsg);
    }

    //Where business happens
    private async Task<ChatCompletionMessage[]> SendChatMessage(
      int userId,
      ChatCompletionMessage message)
    {
        this.StackMessages(userId, message);

        var messagesSend = new List<ChatCompletionMessage>(new[] { this._firstPrompt });
        messagesSend.AddRange(this._userMessages.ContainsKey(userId) ? this._userMessages[userId] : new List<ChatCompletionMessage>());

        const string url = "https://api.openai.com/v1/chat/completions";

        var request = new
        {
            messages = messagesSend.ToArray(),
            model = "gpt-3.5-turbo",
            max_tokens = 100,
            temperature = 0.6
        };

        var requestJson = JsonConvert.SerializeObject(request);
        var requestContent = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
        var httpResponseMessage = await this._openAIClient.PostAsync(url, requestContent);
        var jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeAnonymousType(jsonString, new
        {
            choices = new[] { new { message = new ChatCompletionMessage { Role = string.Empty, Content = string.Empty, Name = "ChatGPT" } } },
            error = new { message = string.Empty }
        });

        if (!string.IsNullOrEmpty(responseObject?.error?.message))  // Check for errors
        {
            return null;
            //responseObject?.error.message
        }

        var messagesGtp = responseObject?.choices.Select(x => x.message).ToArray();

        //stack the response as well - everything is context to Open AI
        this.StackMessages(userId, messagesGtp);

        return messagesGtp;
    }

    public void Dispose()
    {
        this._openAIClient.Dispose();

        GC.SuppressFinalize(this);
    }
}
