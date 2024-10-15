namespace WibboEmulator.Games.Chats.Commands.User.Premium;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class VipProtect : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        session.User.HasPremiumProtect = !session.User.HasPremiumProtect;

        if (session.User.HasPremiumProtect)
        {
            session.SendWhisper(LanguageManager.TryGetValue("cmd.premium.true", session.Language));
        }
        else
        {
            session.SendWhisper(LanguageManager.TryGetValue("cmd.premium.false", session.Language));
        }
    }
}
