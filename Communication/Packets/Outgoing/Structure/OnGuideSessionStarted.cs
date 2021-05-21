namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class OnGuideSessionStarted : ServerPacket
    {
        public OnGuideSessionStarted()
            : base(ServerPacketHeader.OnGuideSessionStarted)
        {

        }
    }
}
