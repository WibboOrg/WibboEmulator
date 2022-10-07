namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RegenMap : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        room.GetGameMap().GenerateMaps();
        session.SendWhisper("Rafraichissement de la map d'appartement");
    }
}
