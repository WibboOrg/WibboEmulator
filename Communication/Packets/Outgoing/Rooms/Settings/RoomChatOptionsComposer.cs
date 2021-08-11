namespace Butterfly.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class RoomChatOptionsComposer : ServerPacket
    {
        public RoomChatOptionsComposer()
            : base(ServerPacketHeader.ROOM_SETTINGS_CHAT)
        {

        }
    }
}
