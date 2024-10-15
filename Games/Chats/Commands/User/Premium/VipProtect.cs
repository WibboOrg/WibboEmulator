namespace WibboEmulator.Games.Chats.Commands.User.Premium;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class VipProtect : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        Session.User.HasPremiumProtect = !Session.User.HasPremiumProtect;

        if (Session.User.HasPremiumProtect)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.premium.true", Session.Language));
        }
        else
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.premium.false", Session.Language));
        }
    }
}
