namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Chats.Logs;
using WibboEmulator.Games.Moderation;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

internal class ModeratorTicketChatlogComposer : ServerPacket
{
    public ModeratorTicketChatlogComposer(ModerationTicket ticket, RoomData roomData, List<ChatlogEntry> chatlogs)
        : base(ServerPacketHeader.CFH_CHATLOG)
    {
        this.WriteInteger(ticket.TicketId);
        this.WriteInteger(ticket.SenderId);
        this.WriteInteger(ticket.ReportedId);
        this.WriteInteger(roomData.Id);

        this.WriteBoolean(false);
        this.WriteInteger(roomData.Id);
        this.WriteString(roomData.Name);

        this.WriteShort(chatlogs.Count);
        foreach (var chat in chatlogs)
        {
            if (chat != null)
            {
                this.WriteString(UnixTimestamp.FromUnixTimestamp(chat.Timestamp).ToShortTimeString()); //this.timeSpoken.Minute
                this.WriteInteger(chat.UserID); //this.timeSpoken.Minute
                this.WriteString(chat.Username);
                this.WriteString(chat.Message);
                this.WriteBoolean(false); // Text is bold
            }
            else
            {
                this.WriteString("0"); //this.timeSpoken.Minute
                this.WriteInteger(0); //this.timeSpoken.Minute
                this.WriteString("");
                this.WriteString("");
                this.WriteBoolean(false); // Text is bold
            }
        }
    }
}
