namespace WibboEmulator.Games.Chats.Commands.User.Premium;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class VipProtect : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        session.User.PremiumProtect = !session.User.PremiumProtect;

        if (session.User.PremiumProtect)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.premium.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.premium.false", session.Langue));
        }
    }
}
