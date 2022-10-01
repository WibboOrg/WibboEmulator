using WibboEmulator.Communication.Packets.Outgoing.Misc;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Users;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class RoomNuxAlertEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            User user = Session.GetUser();

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
}
