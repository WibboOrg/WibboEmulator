using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Notifications;

using Butterfly.Game.Users;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RoomNuxAlert : IPacketEvent
    {
        public void Parse(Game.GameClients.GameClient Session, ClientPacket Packet)
        {
            Habbo habbo = Session.GetHabbo();

            habbo.PassedNuxCount++;

            if (habbo.PassedNuxCount == 2)
            {
                Session.SendPacket(new NuxAlertComposer("helpBubble/add/BOTTOM_BAR_CATALOGUE/nux.bot.info.shop.1"));
            }
            else if (habbo.PassedNuxCount == 3)
            {
                Session.SendPacket(new NuxAlertComposer("helpBubble/add/BOTTOM_BAR_INVENTORY/nux.bot.info.inventory.1"));
            }
            else if (habbo.PassedNuxCount == 4)
            {
                Session.SendPacket(new NuxAlertComposer("helpBubble/add/MEMENU_CLOTHES/nux.bot.info.memenu.1"));
            }
            else if (habbo.PassedNuxCount == 5)
            {
                Session.SendPacket(new NuxAlertComposer("helpBubble/add/CHAT_INPUT/nux.bot.info.chat.1"));
            }
            else if (habbo.PassedNuxCount == 6)
            {
                Session.SendPacket(new NuxAlertComposer("helpBubble/add/CREDITS_BUTTON/nux.bot.info.credits.1"));
            }
            else
            {
                Session.SendPacket(new NuxAlertComposer("nux/lobbyoffer/show"));
                habbo.Nuxenable = false;

                ServerPacket nuxStatus = new ServerPacket(ServerPacketHeader.NuxAlertComposer);
                nuxStatus.WriteInteger(0);
                Session.SendPacket(nuxStatus);
            }
        }
    }
}
