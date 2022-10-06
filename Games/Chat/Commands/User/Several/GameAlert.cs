namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class GameAlert : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        session.ShowGameAlert = !session.ShowGameAlert;

        session.SendWhisper(session.ShowGameAlert ? "Alerte d'animation activée" : "Alerte d'animation désactivée", true);
    }
}
