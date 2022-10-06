namespace WibboEmulator.Communication.Packets.Outgoing.Users;

internal class UpdateUsernameComposer : ServerPacket
{
    public UpdateUsernameComposer(string name)
        : base(ServerPacketHeader.USER_CHANGE_NAME)
    {
        this.WriteInteger(0);
        this.WriteString(name);
        this.WriteInteger(0);
    }

    public UpdateUsernameComposer(string name, int virtualId)
        : base(ServerPacketHeader.USER_CHANGE_NAME)
    {
        this.WriteInteger(virtualId);
        this.WriteString(name);
        this.WriteInteger(1);
        this.WriteString(name);
    }
}
