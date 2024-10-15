namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Nux;
using WibboEmulator.Communication.Packets.Outgoing.Misc;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Games.GameClients;

internal sealed class RoomNuxAlertEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var user = Session.User;

        user.PassedNuxCount++;

        if (user.PassedNuxCount == 2)
        {
            Session.SendPacket(new InClientLinkComposer("helpBubble/add/BOTTOM_BAR_CATALOGUE/nux.bot.info.shop.1"));
        }
        else if (user.PassedNuxCount == 3)
        {
            Session.SendPacket(new InClientLinkComposer("helpBubble/add/BOTTOM_BAR_INVENTORY/nux.bot.info.inventory.1"));
        }
        else if (user.PassedNuxCount == 4)
        {
            Session.SendPacket(new InClientLinkComposer("helpBubble/add/MEMENU_CLOTHES/nux.bot.info.memenu.1"));
        }
        else if (user.PassedNuxCount == 5)
        {
            Session.SendPacket(new InClientLinkComposer("helpBubble/add/CHAT_INPUT/nux.bot.info.chat.1"));
        }
        else if (user.PassedNuxCount == 6)
        {
            Session.SendPacket(new InClientLinkComposer("helpBubble/add/CREDITS_BUTTON/nux.bot.info.credits.1"));
        }
        else
        {
            Session.SendPacket(new InClientLinkComposer("nux/lobbyoffer/show"));
            user.Nuxenable = false;

            Session.SendPacket(new NuxAlertComposer(0));
        }
    }
}
