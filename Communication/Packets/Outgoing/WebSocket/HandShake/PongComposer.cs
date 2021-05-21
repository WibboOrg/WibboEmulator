namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class PongComposer : ServerPacket
    {
        public PongComposer()
            : base(4)
        {
        }
    }
}
