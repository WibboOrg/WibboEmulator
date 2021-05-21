namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class InRoomComposer : ServerPacket
    {
        public InRoomComposer(bool InRoom)
            : base(5)
        {
            this.WriteBoolean(InRoom);
        }
    }
}
