namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class OnGuideSessionAttached : ServerPacket
    {
        public OnGuideSessionAttached()
            : base(ServerPacketHeader.OnGuideSessionAttached)
        {

        }
    }
}
