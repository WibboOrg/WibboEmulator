namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class OnGuideSessionDetached : ServerPacket
    {
        public OnGuideSessionDetached()
            : base(ServerPacketHeader.OnGuideSessionDetached)
        {

        }
    }
}
