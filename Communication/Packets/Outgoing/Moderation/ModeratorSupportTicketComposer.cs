using Butterfly.Game.Moderation;
using Butterfly.Game.Users;
using Butterfly.Utility;
using System;

namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorSupportTicketComposer : ServerPacket
    {
        public ModeratorSupportTicketComposer(ModerationTicket ticket)
            : base(ServerPacketHeader.ISSUE_INFO)
        {
            User userReported = ButterflyEnvironment.GetHabboById(ticket.ReportedId);
            User userSender = ButterflyEnvironment.GetHabboById(ticket.SenderId);
            User userModerator = ButterflyEnvironment.GetHabboById(ticket.ModeratorId);

            WriteInteger(ticket.Id);
            WriteInteger(ticket.TabId);
            WriteInteger(3);
            WriteInteger(ticket.Type);
            WriteInteger((int)(ButterflyEnvironment.GetUnixTimestamp() - ticket.Timestamp) * 1000);
            WriteInteger(ticket.Score);
            WriteInteger(ticket.SenderId);
            WriteInteger(ticket.SenderId);
            if (userSender != null)
            {
                WriteString(ticket.SenderName.Equals("") ? userSender.Username : ticket.SenderName);
            }
            else
            {
                WriteString(ticket.SenderName);
            }

            WriteInteger(ticket.ReportedId);
            if (userReported != null)
            {
                WriteString(ticket.ReportedName.Equals("") ? userReported.Username : ticket.ReportedName);
            }
            else
            {
                WriteString(ticket.ReportedName);
            }

            WriteInteger(ticket.Status == TicketStatusType.PICKED ? ticket.ModeratorId : 0);
            if (userModerator != null)
            {
                WriteString(ticket.Status == TicketStatusType.PICKED ? (ticket.ModName.Equals("") ? userModerator.Username : ticket.ModName) : "");
            }
            else
            {
                WriteString(ticket.ModName);
            }

            WriteString(ticket.Message);
            WriteInteger(0);
            WriteInteger(0);
        }
    }
}
