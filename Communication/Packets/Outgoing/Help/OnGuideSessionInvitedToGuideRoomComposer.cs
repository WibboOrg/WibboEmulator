namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionInvitedToGuideRoom : ServerPacket
    {
        public OnGuideSessionInvitedToGuideRoom()
            : base(ServerPacketHeader.OnGuideSessionInvitedToGuideRoom)
        {

        }
    }
}
