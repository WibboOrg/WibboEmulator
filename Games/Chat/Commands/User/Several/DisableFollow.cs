namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisableFollow : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (session.GetUser().HideInRoom)
        {
            session.GetUser().HideInRoom = false;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.followme.true", session.Langue));
        }
        else
        {
            session.GetUser().HideInRoom = true;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.followme.false", session.Langue));
        }
    }
}
