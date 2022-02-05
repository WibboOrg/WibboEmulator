using Butterfly.Communication.Packets.Outgoing.Misc;
using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Game.Clients;
using Butterfly.Game.Users;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RoomNuxAlertEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            User habbo = Session.GetHabbo();

            habbo.PassedNuxCount++;

            if (habbo.PassedNuxCount == 2)
            {
                Session.SendPacket(new InClientLinkComposer("helpBubble/add/BOTTOM_BAR_CATALOGUE/nux.bot.info.shop.1"));
            }
            else if (habbo.PassedNuxCount == 3)
            {
                Session.SendPacket(new InClientLinkComposer("helpBubble/add/BOTTOM_BAR_INVENTORY/nux.bot.info.inventory.1"));
            }
            else if (habbo.PassedNuxCount == 4)
            {
                Session.SendPacket(new InClientLinkComposer("helpBubble/add/MEMENU_CLOTHES/nux.bot.info.memenu.1"));
            }
            else if (habbo.PassedNuxCount == 5)
            {
                Session.SendPacket(new InClientLinkComposer("helpBubble/add/CHAT_INPUT/nux.bot.info.chat.1"));
            }
            else if (habbo.PassedNuxCount == 6)
            {
                Session.SendPacket(new InClientLinkComposer("helpBubble/add/CREDITS_BUTTON/nux.bot.info.credits.1"));
            }
            else
            {
                Session.SendPacket(new InClientLinkComposer("nux/lobbyoffer/show"));
                habbo.Nuxenable = false;

                Session.SendPacket(new NuxAlertComposer(0));
            }
        }
    }
}
