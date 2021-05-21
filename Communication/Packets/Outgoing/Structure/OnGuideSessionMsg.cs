namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class OnGuideSessionMsg : ServerPacket
    {
        public OnGuideSessionMsg()
            : base(ServerPacketHeader.OnGuideSessionMsg)
        {

        }
    }
}
