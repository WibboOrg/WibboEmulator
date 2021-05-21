namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class OnGuideSessionError : ServerPacket
    {
        public OnGuideSessionError()
            : base(ServerPacketHeader.OnGuideSessionError)
        {

        }
    }
}
