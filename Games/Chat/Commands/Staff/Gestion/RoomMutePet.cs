namespace WibboEmulator.Games.Chat.Commands.Staff.Gestion;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomMutePet : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (Room.RoomMutePets)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.roommutepet.true", session.Langue));
            Room.RoomMutePets = false;
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.roommutepet.false", session.Langue));
            Room.RoomMutePets = true;
        }
    }
}
