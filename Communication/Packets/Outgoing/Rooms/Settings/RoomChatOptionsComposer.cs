namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;

internal sealed class RoomChatOptionsComposer : ServerPacket
{
    public RoomChatOptionsComposer(int chatType, int chatBalloon, int chatSpeed, int chatMaxDistance, int chatFloodProtection)
        : base(ServerPacketHeader.ROOM_SETTINGS_CHAT)
    {
        this.WriteInteger(chatType);
        this.WriteInteger(chatBalloon);
        this.WriteInteger(chatSpeed);
        this.WriteInteger(chatMaxDistance);
        this.WriteInteger(chatFloodProtection);
    }
}
