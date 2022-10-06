namespace WibboEmulator.Games.Chat.Commands.User.Premium;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class VipProtect : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        session.GetUser().PremiumProtect = !session.GetUser().PremiumProtect;

        if (session.GetUser().PremiumProtect)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.premium.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.premium.false", session.Langue));
        }
    }
}
