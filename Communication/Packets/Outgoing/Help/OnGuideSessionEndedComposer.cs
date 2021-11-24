namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionEndedComposer : ServerPacket
    {
        public OnGuideSessionEndedComposer()
            : base(ServerPacketHeader.OnGuideSessionEnded)
        {

        }
    }
}
