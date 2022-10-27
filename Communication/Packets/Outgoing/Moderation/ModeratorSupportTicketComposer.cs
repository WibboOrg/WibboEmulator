namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Moderations;

internal class ModeratorSupportTicketComposer : ServerPacket
{
    public ModeratorSupportTicketComposer(ModerationTicket ticket)
        : base(ServerPacketHeader.ISSUE_INFO)
    {
        var userReported = WibboEnvironment.GetUserById(ticket.ReportedId);
        var userSender = WibboEnvironment.GetUserById(ticket.SenderId);
        var userModerator = WibboEnvironment.GetUserById(ticket.ModeratorId);

        this.WriteInteger(ticket.Id);
        this.WriteInteger(ticket.TabId);
        this.WriteInteger(3);
        this.WriteInteger(ticket.Type);
        this.WriteInteger((int)(WibboEnvironment.GetUnixTimestamp() - ticket.Timestamp) * 1000);
        this.WriteInteger(ticket.Score);
        this.WriteInteger(ticket.SenderId);
        this.WriteInteger(ticket.SenderId);
        if (userSender != null)
        {
            this.WriteString(ticket.SenderName.Equals("") ? userSender.Username : ticket.SenderName);
        }
        else
        {
            this.WriteString(ticket.SenderName);
        }

        this.WriteInteger(ticket.ReportedId);
        if (userReported != null)
        {
            this.WriteString(ticket.ReportedName.Equals("") ? userReported.Username : ticket.ReportedName);
        }
        else
        {
            this.WriteString(ticket.ReportedName);
        }

        this.WriteInteger(ticket.Status == TicketStatusType.Picked ? ticket.ModeratorId : 0);
        if (userModerator != null)
        {
            this.WriteString(ticket.Status == TicketStatusType.Picked ? (ticket.ModName.Equals("") ? userModerator.Username : ticket.ModName) : "");
        }
        else
        {
            this.WriteString(ticket.ModName);
        }

        this.WriteString(ticket.Message);
        this.WriteInteger(0);
        this.WriteInteger(0);
    }
}
