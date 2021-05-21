namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class OnGuideSessionInvitedToGuideRoom : ServerPacket
    {
        public OnGuideSessionInvitedToGuideRoom()
            : base(ServerPacketHeader.OnGuideSessionInvitedToGuideRoom)
        {

        }
    }
}
