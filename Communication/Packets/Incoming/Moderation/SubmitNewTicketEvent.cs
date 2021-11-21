using Butterfly.Communication.Packets.Outgoing.Rooms.Notifications;
using Butterfly.Game.Clients;
using Butterfly.Utility;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SubmitNewTicketEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (ButterflyEnvironment.GetGame().GetModerationManager().UsersHasPendingTicket(Session.GetHabbo().Id))
            {
                return;
            }

            string Message = StringCharFilter.Escape(Packet.PopString());
            int TicketType = Packet.PopInt();
            int ReporterId = Packet.PopInt();
            int RoomId = Packet.PopInt();
            int RepporteurId = Packet.PopInt();

            ButterflyEnvironment.GetGame().GetModerationManager().SendNewTicket(Session, TicketType, ReporterId, Message);
            ButterflyEnvironment.GetGame().GetModerationManager().ApplySanction(Session, ReporterId);
            ButterflyEnvironment.GetGame().GetClientManager().SendMessageStaff(RoomNotificationComposer.SendBubble("mention", "Un nouveau ticket vient d'arriver sur le support"));
        }
    }
}
