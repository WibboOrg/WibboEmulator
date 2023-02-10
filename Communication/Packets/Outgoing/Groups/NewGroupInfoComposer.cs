namespace WibboEmulator.Communication.Packets.Outgoing.Groups;

internal sealed class NewGroupInfoComposer : ServerPacket
{
    public NewGroupInfoComposer(int roomId, int groupId)
        : base(ServerPacketHeader.GROUP_PURCHASED)
    {
        this.WriteInteger(roomId);
        this.WriteInteger(groupId);
    }
}
