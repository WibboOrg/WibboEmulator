using Butterfly.Game.Moderation;

namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorInitComposer : ServerPacket
    {
        public ModeratorInitComposer(List<string> UserPresets, List<string> RoomPresets, List<ModerationTicket> Tickets)
            : base(ServerPacketHeader.MODERATION_TOOL)
        {
            WriteInteger(Tickets.Count);
            foreach (ModerationTicket Ticket in Tickets)
            {
                WriteInteger(Ticket.Id); // id
                WriteInteger(Ticket.TabId); // state
                WriteInteger(4); // type (3 or 4 for new style)
                WriteInteger(Ticket.Type); // priority
                WriteInteger((int)(ButterflyEnvironment.GetUnixTimestamp() - Ticket.Timestamp) * 1000); // -->> timestamp
                WriteInteger(Ticket.Score); // priority
                WriteInteger(Ticket.SenderId);
                WriteInteger(Ticket.SenderId); // sender id 8 ints
                WriteString(Ticket.SenderName); // sender name
                WriteInteger(Ticket.ReportedId);
                WriteString(Ticket.ReportedName);
                WriteInteger((Ticket.Status == TicketStatusType.PICKED) ? Ticket.ModeratorId : 0); // mod id
                WriteString(Ticket.ModName); // mod name
                WriteString(Ticket.Message); // issue message
                WriteInteger(0);
                WriteInteger(0);
            }

            WriteInteger(UserPresets.Count);
            foreach (string pre in UserPresets)
            {
                WriteString(pre);
            }

            WriteInteger(0);
            {
                //Loop a string.
            }

            WriteBoolean(true);
            WriteBoolean(true);
            WriteBoolean(true);
            WriteBoolean(true);
            WriteBoolean(true);
            WriteBoolean(true);
            WriteBoolean(true);

            WriteInteger(RoomPresets.Count);
            foreach (string pre in RoomPresets)
            {
                WriteString(pre);
            }
        }
    }
}
