namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionRequesterRoomComposer : ServerPacket
    {
        public OnGuideSessionRequesterRoomComposer()
            : base(ServerPacketHeader.OnGuideSessionRequesterRoom)
        {

        }
    }
}
