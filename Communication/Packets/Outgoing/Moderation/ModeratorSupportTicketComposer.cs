using Wibbo.Game.Moderation;
using Wibbo.Game.Users;

namespace Wibbo.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorSupportTicketComposer : ServerPacket
    {
        public ModeratorSupportTicketComposer(ModerationTicket ticket)
            : base(ServerPacketHeader.ISSUE_INFO)
        {
            User userReported = WibboEnvironment.GetUserById(ticket.ReportedId);
            User userSender = WibboEnvironment.GetUserById(ticket.SenderId);
            User userModerator = WibboEnvironment.GetUserById(ticket.ModeratorId);

            WriteInteger(ticket.Id);
            WriteInteger(ticket.TabId);
            WriteInteger(3);
            WriteInteger(ticket.Type);
            WriteInteger((int)(WibboEnvironment.GetUnixTimestamp() - ticket.Timestamp) * 1000);
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
