namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionDetachedComposer : ServerPacket
    {
        public OnGuideSessionDetachedComposer()
            : base(ServerPacketHeader.GUIDE_SESSION_DETACHED)
        {

        }
    }
}
