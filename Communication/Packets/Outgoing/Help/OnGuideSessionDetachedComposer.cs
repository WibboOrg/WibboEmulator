namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionDetachedComposer : ServerPacket
    {
        public OnGuideSessionDetachedComposer()
            : base(ServerPacketHeader.OnGuideSessionDetached)
        {

        }
    }
}
