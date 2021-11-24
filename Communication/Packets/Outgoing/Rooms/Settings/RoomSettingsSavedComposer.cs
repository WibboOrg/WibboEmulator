namespace Butterfly.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class RoomSettingsSavedComposer : ServerPacket
    {
        public RoomSettingsSavedComposer()
            : base(ServerPacketHeader.ROOM_SETTINGS_SAVE)
        {

        }
    }
}
