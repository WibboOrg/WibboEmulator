namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class GameAlert : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        session.ShowGameAlert = !session.ShowGameAlert;

        session.SendWhisper(session.ShowGameAlert ? "Alerte d'animation activée" : "Alerte d'animation désactivée", true);
    }
}
