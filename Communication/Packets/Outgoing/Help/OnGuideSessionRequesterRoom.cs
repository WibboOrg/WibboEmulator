namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionRequesterRoom : ServerPacket
    {
        public OnGuideSessionRequesterRoom()
            : base(ServerPacketHeader.OnGuideSessionRequesterRoom)
        {

        }
    }
}
