namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class OnGuideSessionRequesterRoom : ServerPacket
    {
        public OnGuideSessionRequesterRoom()
            : base(ServerPacketHeader.OnGuideSessionRequesterRoom)
        {

        }
    }
}
