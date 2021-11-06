namespace Butterfly.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class RoomSettingsSavedMessageComposer : ServerPacket
    {
        public RoomSettingsSavedMessageComposer(int roomID)
            : base(ServerPacketHeader.ROOM_SETTINGS_SAVE)
        {
            WriteInteger(roomID);

        }
    }
}
