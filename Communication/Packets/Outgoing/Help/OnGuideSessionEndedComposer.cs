namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionEnded : ServerPacket
    {
        public OnGuideSessionEnded()
            : base(ServerPacketHeader.OnGuideSessionEnded)
        {

        }
    }
}
