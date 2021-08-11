namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionAttached : ServerPacket
    {
        public OnGuideSessionAttached()
            : base(ServerPacketHeader.OnGuideSessionAttached)
        {

        }
    }
}
