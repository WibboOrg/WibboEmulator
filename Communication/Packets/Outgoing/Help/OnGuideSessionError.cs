namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionError : ServerPacket
    {
        public OnGuideSessionError()
            : base(ServerPacketHeader.OnGuideSessionError)
        {

        }
    }
}
