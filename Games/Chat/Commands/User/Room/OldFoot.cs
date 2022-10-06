namespace WibboEmulator.Games.Chat.Commands.User.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class OldFoot : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
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