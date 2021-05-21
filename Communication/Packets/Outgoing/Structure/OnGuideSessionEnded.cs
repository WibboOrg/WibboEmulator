namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class OnGuideSessionEnded : ServerPacket
    {
        public OnGuideSessionEnded()
            : base(ServerPacketHeader.OnGuideSessionEnded)
        {

        }
    }
}
