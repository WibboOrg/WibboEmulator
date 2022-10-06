namespace WibboEmulator.Communication.Packets.Outgoing.Users;

internal class NameChangeUpdateComposer : ServerPacket
{
    public NameChangeUpdateComposer(string name)
        : base(ServerPacketHeader.CHECK_USER_NAME)
    {
        switch (NameAvailable(name))
        {
            case -2:
                this.WriteInteger(4);
                this.WriteString(name);
                this.WriteInteger(0);
                break;
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

    private static int NameAvailable(string username)
    {
        username = username.ToLower();

        if (username.Length > 15)
        {
            return -2;
        }

        if (username.Length < 3)
        {
            return -2;
        }

        if (!WibboEnvironment.IsValidAlphaNumeric(username))
        {
            return -1;
        }

        return WibboEnvironment.UsernameExists(username) ? 0 : 1;
    }
}
