namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;

internal class UserNameChangeComposer : ServerPacket
{
    public UserNameChangeComposer(string name, int virtualId)
        : base(ServerPacketHeader.UNIT_CHANGE_NAME)
    {
        this.WriteInteger(0);
        this.WriteInteger(virtualId);
        this.WriteString(name);
    }
}
