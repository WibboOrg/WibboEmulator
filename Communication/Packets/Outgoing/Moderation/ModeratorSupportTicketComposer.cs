namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Moderations;
using WibboEmulator.Games.Users;

internal sealed class ModeratorSupportTicketComposer : ServerPacket
{
    public ModeratorSupportTicketComposer(ModerationTicket ticket)
        : base(ServerPacketHeader.ISSUE_INFO)
    {
        var userReported = UserManager.GetUsernameById(ticket.ReportedId);
        var userSender = UserManager.GetUsernameById(ticket.SenderId);
        var userModerator = UserManager.GetUsernameById(ticket.ModeratorId);

        this.WriteInteger(ticket.Id);
        this.WriteInteger(ticket.TabId);
        this.WriteInteger(3);
        this.WriteInteger(ticket.Type);
        this.WriteInteger((int)(WibboEnvironment.GetUnixTimestamp() - ticket.Timestamp) * 1000);
        this.WriteInteger(ticket.Score);
        this.WriteInteger(ticket.SenderId);
        this.WriteInteger(ticket.SenderId);
        if (userSender != string.Empty)
        {
            this.WriteString(ticket.SenderName == string.Empty ? userSender : ticket.SenderName);
        }
        else
        {
            this.WriteString(ticket.SenderName);
        }

        this.WriteInteger(ticket.ReportedId);
        if (userReported != string.Empty)
        {
            this.WriteString(ticket.ReportedName == string.Empty ? userReported : ticket.ReportedName);
        }
        else
        {
            this.WriteString(ticket.ReportedName);
        }

        this.WriteInteger(ticket.Status == TicketStatusType.Picked ? ticket.ModeratorId : 0);
        if (userModerator != string.Empty)
        {
            this.WriteString(ticket.Status == TicketStatusType.Picked ? (ticket.ModName == string.Empty ? userModerator : ticket.ModName) : "");
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
