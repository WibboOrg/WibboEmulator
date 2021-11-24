namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionErrorComposer : ServerPacket
    {
        public OnGuideSessionErrorComposer()
            : base(ServerPacketHeader.OnGuideSessionError)
        {

        }
    }
}
