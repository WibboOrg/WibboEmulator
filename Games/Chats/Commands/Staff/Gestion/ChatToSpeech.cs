namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Core;
using WibboEmulator.Core.Language;
using WibboEmulator.Core.OpenIA;
using WibboEmulator.Core.Settings;
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

        if (!OpenAIProxy.IsReadyToSendAudio)
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

        var audioBinary = OpenAIProxy.TextToSpeech(nameVoice, text).GetAwaiter().GetResult();

        var audioName = $"{session.User.Id}_{room.Id}_{Guid.NewGuid()}";

        var audioId = UploadApi.ChatAudio(audioBinary, audioName);

        if (string.IsNullOrEmpty(audioId) || audioName != audioId)
        {
            session.SendNotification(LanguageManager.TryGetValue("notif.error", session.Language));
            return;
        }

        var audioPath = $"/chat-audio/{audioName}.webm";

        var audioUploadUrl = SettingsManager.GetData<string>("audio.upload.url");

        var basePath = new Uri(audioUploadUrl).GetLeftPart(UriPartial.Authority);

        var audioUrl = $"{basePath}{audioPath}";

        session.User.ChatMessageManager.AddMessage(session.User.Id, session.User.Username, room.Id, audioUrl, UnixTimestamp.GetNow());
        room.ChatlogManager.AddMessage(session.User.Id, session.User.Username, room.Id, audioUrl, UnixTimestamp.GetNow());

        userRoom.OnChatAudio(audioPath);
    }
}
