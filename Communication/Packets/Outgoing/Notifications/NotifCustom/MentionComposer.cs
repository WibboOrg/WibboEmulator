namespace WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;

internal class MentionComposer : ServerPacket
{
    public MentionComposer(int userId, string username, string look, string msg)
     : base(ServerPacketHeader.MENTION)
    {
        this.WriteInteger(userId);
        this.WriteString(username);
        this.WriteString(look);
        this.WriteString(msg);
    }
}
