namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;

internal class RoomChatOptionsComposer : ServerPacket
{
    public RoomChatOptionsComposer(int ChatType, int ChatBalloon, int ChatSpeed, int ChatMaxDistance, int ChatFloodProtection)
        : base(ServerPacketHeader.ROOM_SETTINGS_CHAT)
    {
        this.WriteInteger(ChatType);
        this.WriteInteger(ChatBalloon);
        this.WriteInteger(ChatSpeed);
        this.WriteInteger(ChatMaxDistance);
        this.WriteInteger(ChatFloodProtection);
    }
}
