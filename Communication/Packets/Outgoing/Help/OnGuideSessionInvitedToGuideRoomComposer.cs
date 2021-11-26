namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionInvitedToGuideRoomComposer : ServerPacket
    {
        public OnGuideSessionInvitedToGuideRoomComposer(int roomId, string name)
            : base(ServerPacketHeader.OnGuideSessionInvitedToGuideRoom)
        {
            WriteInteger(roomId);
            WriteString(name);
        }
    }
}
