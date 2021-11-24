namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionAttachedComposer : ServerPacket
    {
        public OnGuideSessionAttachedComposer()
            : base(ServerPacketHeader.OnGuideSessionAttached)
        {

        }
    }
}
