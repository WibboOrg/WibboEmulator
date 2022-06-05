namespace Wibbo.Communication.Packets.Outgoing.Notifications.NotifCustom
{
    internal class MentionComposer : ServerPacket
    {
        public MentionComposer(int UserId, string Username, string Look, string Msg)
         : base(ServerPacketHeader.MENTION)
        {
            this.WriteInteger(UserId);
            this.WriteString(Username);
            this.WriteString(Look);
            this.WriteString(Msg);
        }
    }
}
