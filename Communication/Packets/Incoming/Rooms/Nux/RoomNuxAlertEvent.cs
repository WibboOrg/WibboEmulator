namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Nux;
using WibboEmulator.Communication.Packets.Outgoing.Misc;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Games.GameClients;

internal class RoomNuxAlertEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var user = session.User;

        user.PassedNuxCount++;

        if (user.PassedNuxCount == 2)
        {
            session.SendPacket(new InClientLinkComposer("helpBubble/add/BOTTOM_BAR_CATALOGUE/nux.bot.info.shop.1"));
        }
        else if (user.PassedNuxCount == 3)
        {
            session.SendPacket(new InClientLinkComposer("helpBubble/add/BOTTOM_BAR_INVENTORY/nux.bot.info.inventory.1"));
        }
        else if (user.PassedNuxCount == 4)
        {
            session.SendPacket(new InClientLinkComposer("helpBubble/add/MEMENU_CLOTHES/nux.bot.info.memenu.1"));
        }
        else if (user.PassedNuxCount == 5)
        {
            session.SendPacket(new InClientLinkComposer("helpBubble/add/CHAT_INPUT/nux.bot.info.chat.1"));
        }
        else if (user.PassedNuxCount == 6)
        {
            session.SendPacket(new InClientLinkComposer("helpBubble/add/CREDITS_BUTTON/nux.bot.info.credits.1"));
        }
        else
        {
            session.SendPacket(new InClientLinkComposer("nux/lobbyoffer/show"));
            user.Nuxenable = false;

            session.SendPacket(new NuxAlertComposer(0));
        }
    }
}
