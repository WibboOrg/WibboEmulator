namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;

internal sealed class RoomSettingsSavedComposer : ServerPacket
{
    public RoomSettingsSavedComposer(int roomId)
        : base(ServerPacketHeader.ROOM_SETTINGS_SAVE) => this.WriteInteger(roomId);
}
