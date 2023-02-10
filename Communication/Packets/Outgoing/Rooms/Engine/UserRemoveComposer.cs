namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;

internal sealed class UserRemoveComposer : ServerPacket
{
    public UserRemoveComposer(int userId)
        : base(ServerPacketHeader.UNIT_REMOVE) => this.WriteString(userId.ToString());
}
