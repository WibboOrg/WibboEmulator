namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionDetached : ServerPacket
    {
        public OnGuideSessionDetached()
            : base(ServerPacketHeader.OnGuideSessionDetached)
        {

        }
    }
}
