namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionInvitedToGuideRoomComposer : ServerPacket
    {
        public OnGuideSessionInvitedToGuideRoomComposer()
            : base(ServerPacketHeader.OnGuideSessionInvitedToGuideRoom)
        {

        }
    }
}
