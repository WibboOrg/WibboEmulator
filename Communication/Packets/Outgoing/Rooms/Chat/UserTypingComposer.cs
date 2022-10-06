namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;

internal class UserTypingComposer : ServerPacket
{
    public UserTypingComposer(int virtualId, bool typing)
        : base(ServerPacketHeader.UNIT_TYPING)
    {
        this.WriteInteger(virtualId);
        this.WriteInteger(typing ? 1 : 0);
    }
}
