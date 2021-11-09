namespace Butterfly.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class RoomSettingsSavedMessageComposer : ServerPacket
    {
        public RoomSettingsSavedMessageComposer()
            : base(ServerPacketHeader.ROOM_SETTINGS_SAVE)
        {

        }
    }
}
