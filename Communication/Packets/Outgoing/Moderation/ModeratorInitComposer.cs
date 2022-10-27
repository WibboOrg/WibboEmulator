namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Moderations;

internal class ModeratorInitComposer : ServerPacket
{
    public ModeratorInitComposer(List<string> userPresets, List<string> roomPresets, List<ModerationTicket> tickets)
        : base(ServerPacketHeader.MODERATION_TOOL)
    {
        this.WriteInteger(tickets.Count);
        foreach (var ticket in tickets)
        {
            this.WriteInteger(ticket.Id); // id
            this.WriteInteger(ticket.TabId); // state
            this.WriteInteger(4); // type (3 or 4 for new style)
            this.WriteInteger(ticket.Type); // priority
            this.WriteInteger((int)(WibboEnvironment.GetUnixTimestamp() - ticket.Timestamp) * 1000); // -->> timestamp
            this.WriteInteger(ticket.Score); // priority
            this.WriteInteger(ticket.SenderId);
            this.WriteInteger(ticket.SenderId); // sender id 8 ints
            this.WriteString(ticket.SenderName); // sender name
            this.WriteInteger(ticket.ReportedId);
            this.WriteString(ticket.ReportedName);
            this.WriteInteger((ticket.Status == TicketStatusType.Picked) ? ticket.ModeratorId : 0); // mod id
            this.WriteString(ticket.ModName); // mod name
            this.WriteString(ticket.Message); // issue message
            this.WriteInteger(0);
            this.WriteInteger(0);
        }

        this.WriteInteger(userPresets.Count);
        foreach (var pre in userPresets)
        {
            this.WriteString(pre);
        }

        this.WriteInteger(0);
        {
            //Loop a string.
        }

        this.WriteBoolean(true);
        this.WriteBoolean(true);
        this.WriteBoolean(true);
        this.WriteBoolean(true);
        this.WriteBoolean(true);
        this.WriteBoolean(true);
        this.WriteBoolean(true);

        this.WriteInteger(roomPresets.Count);
        foreach (var pre in roomPresets)
        {
            this.WriteString(pre);
        }
    }
}
