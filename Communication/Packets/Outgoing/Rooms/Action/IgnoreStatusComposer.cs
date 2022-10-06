namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Action;

internal class IgnoreStatusComposer : ServerPacket
{
    public IgnoreStatusComposer(int statue, string name)
        : base(ServerPacketHeader.USER_IGNORED_RESULT)
    {
        this.WriteInteger(statue);
        this.WriteString(name);
    }
}
