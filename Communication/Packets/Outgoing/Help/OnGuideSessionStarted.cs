namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionStarted : ServerPacket
    {
        public OnGuideSessionStarted()
            : base(ServerPacketHeader.OnGuideSessionStarted)
        {

        }
    }
}
