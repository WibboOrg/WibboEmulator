namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class RoomChatOptionsComposer : ServerPacket
    {
        public RoomChatOptionsComposer()
            : base(ServerPacketHeader.ROOM_SETTINGS_CHAT)
        {

        }
    }
}
