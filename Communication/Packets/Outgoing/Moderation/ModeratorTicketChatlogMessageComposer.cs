using Butterfly.HabboHotel.Moderation;
using Butterfly.HabboHotel.Rooms;
using Butterfly.Utilities;

namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorTicketChatlogMessageComposer : ServerPacket
    {
        public ModeratorTicketChatlogMessageComposer(ModerationTicket Ticket, RoomData roomData, double Timestamp)
            : base(ServerPacketHeader.ModeratorTicketChatlogMessageComposer)
        {
            WriteInteger(Ticket.Id);
            WriteInteger(Ticket.Sender != null ? Ticket.Sender.Id : 0);
            WriteInteger(Ticket.Reported != null ? Ticket.Reported.Id : 0);
            WriteInteger(roomData.Id);

            WriteByte(1);
            WriteShort(2);
            WriteString("roomname");
            WriteByte(2);
            WriteString(roomData.Name);
            WriteString("roomId");
            WriteByte(1);
            WriteInteger(roomData.Id);

            WriteShort(Ticket.ReportedChats.Count);
            foreach (string Chat in Ticket.ReportedChats)
            {
                WriteString(UnixTimestamp.FromUnixTimestamp(Timestamp).ToShortDateString());
                WriteInteger(Ticket.Id);
                WriteString(Ticket.Reported != null ? Ticket.Reported.Username : "Aucun pseudonyme ?");
                WriteString(Chat);
                WriteBoolean(false);
            }
        }
    }
}
