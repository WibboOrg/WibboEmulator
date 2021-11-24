namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionStartedComposer : ServerPacket
    {
        public OnGuideSessionStartedComposer()
            : base(ServerPacketHeader.OnGuideSessionStarted)
        {

        }
    }
}
