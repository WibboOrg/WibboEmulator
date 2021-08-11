namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionMsg : ServerPacket
    {
        public OnGuideSessionMsg()
            : base(ServerPacketHeader.OnGuideSessionMsg)
        {

        }
    }
}
