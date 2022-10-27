namespace WibboEmulator.Games.Chats.Commands.User.Build;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class UseStop : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        session.User.ForceUse = -1;

        session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.use.disabled", session.Langue));
    }
}
