namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class RoomSettingsSavedMessageComposer : ServerPacket
    {
        public RoomSettingsSavedMessageComposer()
            : base(ServerPacketHeader.ROOM_SETTINGS_SAVE)
        {

        }
    }
}
