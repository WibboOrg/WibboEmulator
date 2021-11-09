using System;
using System.Collections.Generic;
using Butterfly.HabboHotel.Moderation;
using Butterfly.Utilities;

namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorInitMessageComposer : ServerPacket
    {
        public ModeratorInitMessageComposer(ICollection<string> UserPresets, ICollection<string> RoomPresets, ICollection<ModerationTicket> Tickets)
            : base(ServerPacketHeader.MODERATION_TOOL)
        {
            WriteInteger(Tickets.Count);
            foreach (ModerationTicket Ticket in Tickets)
            {
                WriteInteger(Ticket.Id);
                WriteInteger(Ticket.GetStatus(Id)); 
                WriteInteger(Ticket.Type); 
                WriteInteger(Ticket.Category); 
                WriteInteger(Convert.ToInt32((DateTime.Now - UnixTimestamp.FromUnixTimestamp(Ticket.Timestamp)).TotalMilliseconds));
                WriteInteger(Ticket.Priority);
                WriteInteger(Ticket.Sender == null ? 0 : Ticket.Sender.Id); 
                WriteInteger(1);
                WriteString(Ticket.Sender == null ? string.Empty : Ticket.Sender.Username);
                WriteInteger(Ticket.Reported == null ? 0 : Ticket.Reported.Id); 
                WriteString(Ticket.Reported == null ? string.Empty : Ticket.Reported.Username); 
                WriteInteger(Ticket.Moderator == null ? 0 : Ticket.Moderator.Id);
                WriteString(Ticket.Moderator == null ? string.Empty : Ticket.Moderator.Username);
                WriteString(Ticket.Issue);
                WriteInteger(Ticket.Room == null ? 0 : Ticket.Room.Id);
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
