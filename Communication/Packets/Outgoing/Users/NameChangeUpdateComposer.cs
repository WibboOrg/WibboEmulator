namespace WibboEmulator.Communication.Packets.Outgoing.Users;

using WibboEmulator.Games.Users;

internal sealed class NameChangeUpdateComposer : ServerPacket
{
    public NameChangeUpdateComposer(string name)
        : base(ServerPacketHeader.CHECK_USER_NAME)
    {
        switch (UserManager.UsernameAvailable(name))
        {
            case -1:
                this.WriteInteger(4);
                this.WriteString(name);
                this.WriteInteger(0);
                break;
            case 0:
                this.WriteInteger(5);
                this.WriteString(name);
                this.WriteInteger(2);
                this.WriteString("--" + name + "--");
                this.WriteString("Xx" + name + "xX");
                break;
            default:
                this.WriteInteger(0);
                this.WriteString(name);
                this.WriteInteger(0);
                break;
        }
    }
}
