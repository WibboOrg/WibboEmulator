namespace Butterfly.Communication.Packets.Outgoing.Notifications.NotifCustom
{
    internal class NotifTopComposer : ServerPacket
    {
        public NotifTopComposer(string Message)
         : base(ServerPacketHeader.NOTIF_TOP)
        {
            this.WriteString(Message);
        }
    }
}
