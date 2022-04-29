namespace Butterfly.Communication.Packets.Outgoing.WebSocket
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
