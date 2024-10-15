namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomMutePet : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (room.RoomMutePets)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.roommutepet.true", Session.Language));
            room.RoomMutePets = false;
        }
        else
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.roommutepet.false", Session.Language));
            room.RoomMutePets = true;
        }
    }
}
