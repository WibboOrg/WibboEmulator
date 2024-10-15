namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomMutePet : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (room.RoomMutePets)
        {
            session.SendWhisper(LanguageManager.TryGetValue("cmd.roommutepet.true", session.Language));
            room.RoomMutePets = false;
        }
        else
        {
            session.SendWhisper(LanguageManager.TryGetValue("cmd.roommutepet.false", session.Language));
            room.RoomMutePets = true;
        }
    }
}
