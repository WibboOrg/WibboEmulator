namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Moderation;

internal class ModeratorInitComposer : ServerPacket
{
    public ModeratorInitComposer(List<string> UserPresets, List<string> RoomPresets, List<ModerationTicket> Tickets)
        : base(ServerPacketHeader.MODERATION_TOOL)
    {
        this.WriteInteger(Tickets.Count);
        foreach (var Ticket in Tickets)
        {
            this.WriteInteger(Ticket.Id); // id
            this.WriteInteger(Ticket.TabId); // state
            this.WriteInteger(4); // type (3 or 4 for new style)
            this.WriteInteger(Ticket.Type); // priority
            this.WriteInteger((int)(WibboEnvironment.GetUnixTimestamp() - Ticket.Timestamp) * 1000); // -->> timestamp
            this.WriteInteger(Ticket.Score); // priority
            this.WriteInteger(Ticket.SenderId);
            this.WriteInteger(Ticket.SenderId); // sender id 8 ints
            this.WriteString(Ticket.SenderName); // sender name
            this.WriteInteger(Ticket.ReportedId);
            this.WriteString(Ticket.ReportedName);
            this.WriteInteger((Ticket.Status == TicketStatusType.PICKED) ? Ticket.ModeratorId : 0); // mod id
            this.WriteString(Ticket.ModName); // mod name
            this.WriteString(Ticket.Message); // issue message
            this.WriteInteger(0);
            this.WriteInteger(0);
        }

        this.WriteInteger(UserPresets.Count);
        foreach (var pre in UserPresets)
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

        this.WriteInteger(RoomPresets.Count);
        foreach (var pre in RoomPresets)
        {
            this.WriteString(pre);
        }
    }
}
