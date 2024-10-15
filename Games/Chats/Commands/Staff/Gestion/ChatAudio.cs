namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ChatAudio : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var audioName = parameters[1];

        if (string.IsNullOrEmpty(audioName))
        {
            return;
        }

        var audioPath = $"/sounds/{audioName}.mp3";
        userRoom.OnChatAudio(audioPath);
    }
}
