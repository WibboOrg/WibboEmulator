namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionRequesterRoomComposer : ServerPacket
    {
        public OnGuideSessionRequesterRoomComposer(int roomId)
            : base(ServerPacketHeader.OnGuideSessionRequesterRoom)
        {
            WriteInteger(roomId);
        }
    }
}
