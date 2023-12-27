namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

internal sealed class ChatToSpeech : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var nameVoice = parameters[1];
        var text = CommandManager.MergeParams(parameters, 2);

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (WibboEnvironment.GetChatOpenAI().IsReadyToSendAudio() == false)
        {
            userRoom.SendWhisperChat("L'api n'est pas encore disponible");
            return;
        }

        var voiceActors = new List<string> { "alloy", "echo", "fable", "onyx", "nova", "shimmer" };
        if (!voiceActors.Contains(nameVoice))
        {
            userRoom.SendWhisperChat("Veuillez entrer un acteur valide");
            return;
        }

        var audioBinary = WibboEnvironment.GetChatOpenAI().TextToSpeech(nameVoice, text).GetAwaiter().GetResult();

        var audioName = $"{session.User.Id}_{room.Id}_{Guid.NewGuid()}";

        var content = new MultipartFormDataContent("Upload")
        {
            { new StreamContent(new MemoryStream(audioBinary)), "audio", audioName }
        };

        var audioUploadUrl = WibboEnvironment.GetSettings().GetData<string>("audio.upload.url");

        var response = WibboEnvironment.GetHttpClient().PostAsync(audioUploadUrl, content).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", session.Langue));
            return;
        }

        var audioId = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        if (string.IsNullOrEmpty(audioId) || audioName != audioId)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", session.Langue));
            return;
        }

        var audioPath = $"/chat-audio/{audioName}.webm";

        var basePath = new Uri(audioUploadUrl).GetLeftPart(UriPartial.Authority);

        var audioUrl = $"{basePath}{audioPath}";

        session.User.ChatMessageManager.AddMessage(session.User.Id, session.User.Username, room.Id, audioUrl, UnixTimestamp.GetNow());
        room.ChatlogManager.AddMessage(session.User.Id, session.User.Username, room.Id, audioUrl, UnixTimestamp.GetNow());

        userRoom.OnChatAudio(audioPath);
    }
}
