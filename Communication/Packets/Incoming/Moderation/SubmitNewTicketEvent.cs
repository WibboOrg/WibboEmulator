using Wibbo.Communication.Packets.Outgoing.Rooms.Notifications;
using Wibbo.Game.Clients;
using Wibbo.Utilities;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class SubmitNewTicketEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (WibboEnvironment.GetGame().GetModerationManager().UsersHasPendingTicket(Session.GetUser().Id))
            {
                return;
            }

            string Message = StringCharFilter.Escape(Packet.PopString());
            int TicketType = Packet.PopInt();
            int ReporterId = Packet.PopInt();
            int RoomId = Packet.PopInt();
            int chatEntriesCount = Packet.PopInt();
            //chatEntries = Packet.PopString();

            WibboEnvironment.GetGame().GetModerationManager().SendNewTicket(Session, TicketType, ReporterId, Message);
            WibboEnvironment.GetGame().GetModerationManager().ApplySanction(Session, ReporterId);
            WibboEnvironment.GetGame().GetClientManager().SendMessageStaff(RoomNotificationComposer.SendBubble("mention", "Un nouveau ticket vient d'arriver sur le support"));
        }
    }
}
