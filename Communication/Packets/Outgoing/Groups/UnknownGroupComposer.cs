namespace WibboEmulator.Communication.Packets.Outgoing.Groups;

internal sealed class UnknownGroupComposer : ServerPacket
{
    public UnknownGroupComposer(int groupId, int userId)
        : base(ServerPacketHeader.GROUP_MEMBERS_REFRESH)
    {
        this.WriteInteger(groupId);
        this.WriteInteger(userId);
    }
}
