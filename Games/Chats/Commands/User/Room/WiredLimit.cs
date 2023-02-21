namespace WibboEmulator.Games.Chats.Commands.User.Room;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class WiredLimit : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        room.WiredHandler.ToggleSecurityEnabled();

        if (room.WiredHandler.SecurityEnabled())
        {
            userRoom.SendWhisperChat("Attention tu as désactiver la sécurité des wireds. Une mauvaise utilisation de cette commande pourrait détruire ton appartement.");
        }
        else
        {
            userRoom.SendWhisperChat("La sécurité des wireds a été réactiver");
        }
    }
}
