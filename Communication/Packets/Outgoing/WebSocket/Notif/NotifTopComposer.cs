namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class NotifTopComposer : ServerPacket
    {
        public NotifTopComposer(string Message)
         : base(18)
        {
            this.WriteString(Message);
        }
    }
}
