namespace WibboEmulator.Games.Chat.Commands;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class OldFoot : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        Room.OldFoot = !Room.OldFoot;

        if (Room.OldFoot)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.oldfoot.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.oldfoot.false", session.Langue));
        }
    }
}