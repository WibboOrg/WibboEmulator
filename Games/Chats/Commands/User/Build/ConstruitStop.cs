namespace WibboEmulator.Games.Chat.Commands.User.Build;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ConstruitStop : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        userRoom.ConstruitEnable = false;

        session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.construit.disabled", session.Langue));
    }
}
