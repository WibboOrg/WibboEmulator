namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Core;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

internal sealed class ChatToSpeechElevenlabs : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var nameVoice = parameters[1];
        var text = CommandManager.MergeParams(parameters, 2);

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (WibboEnvironment.GetElevenLabs().IsReadyToSendAudio() == false)
        {
            userRoom.SendWhisperChat("L'api n'est pas encore disponible");
            return;
        }

        var voiceActors = new Dictionary<string, string>
        {
            { "callum", "N2lVS1w4EtoT3dr4eOWO" },
            { "nicole", "piTKgcLEGmPE4e6mEKli" }
        };

        if (!voiceActors.TryGetValue(nameVoice, out var modelId))
        {
            userRoom.SendWhisperChat("Veuillez entrer un acteur valide");
            return;
        }

        var audioBinary = WibboEnvironment.GetElevenLabs().TextToSpeech(modelId, text).GetAwaiter().GetResult();

        var audioName = $"{session.User.Id}_{room.Id}_{Guid.NewGuid()}";

        var audioId = UploadApi.ChatAudio(audioBinary, audioName);

        if (string.IsNullOrEmpty(audioId) || audioName != audioId)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", session.Langue));
            return;
        }

        var audioPath = $"/chat-audio/{audioName}.webm";

        var audioUploadUrl = WibboEnvironment.GetSettings().GetData<string>("audio.upload.url");
        var basePath = new Uri(audioUploadUrl).GetLeftPart(UriPartial.Authority);

        var audioUrl = $"{basePath}{audioPath}";

        session.User.ChatMessageManager.AddMessage(session.User.Id, session.User.Username, room.Id, audioUrl, UnixTimestamp.GetNow());
        room.ChatlogManager.AddMessage(session.User.Id, session.User.Username, room.Id, audioUrl, UnixTimestamp.GetNow());

        userRoom.OnChatAudio(audioPath);
    }
}
