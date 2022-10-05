namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisableWhispers : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (session.GetUser().ViewMurmur)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.murmur.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.murmur.false", session.Langue));
        }

        session.GetUser().ViewMurmur = !session.GetUser().ViewMurmur;
    }
}