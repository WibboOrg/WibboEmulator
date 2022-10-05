namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RegenMap : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        Room.GetGameMap().GenerateMaps();
        session.SendWhisper("Rafraichissement de la map d'appartement");
    }
}
