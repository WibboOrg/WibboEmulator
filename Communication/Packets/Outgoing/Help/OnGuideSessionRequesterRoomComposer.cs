namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionRequesterRoomComposer : ServerPacket
    {
        public OnGuideSessionRequesterRoomComposer(int roomId)
            : base(ServerPacketHeader.GUIDE_SESSION_REQUESTER_ROOM)
        {
            WriteInteger(roomId);
        }
    }
}
