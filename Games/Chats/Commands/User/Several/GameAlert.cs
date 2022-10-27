namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class GameAlert : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        session.ShowGameAlert = !session.ShowGameAlert;

        session.SendWhisper(session.ShowGameAlert ? "Alerte d'animation activée" : "Alerte d'animation désactivée", true);
    }
}
