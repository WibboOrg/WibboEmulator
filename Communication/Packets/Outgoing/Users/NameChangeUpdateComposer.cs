namespace Wibbo.Communication.Packets.Outgoing.Users
{
    internal class NameChangeUpdateComposer : ServerPacket
    {
        public NameChangeUpdateComposer(string Name)
            : base(ServerPacketHeader.CHECK_USER_NAME)
        {
            switch (this.NameAvailable(Name))
            {
                case -2:
                    this.WriteInteger(4);
                    this.WriteString(Name);
                    this.WriteInteger(0);
                    break;
                case -1:
                    this.WriteInteger(4);
                    this.WriteString(Name);
                    this.WriteInteger(0);
                    break;
                case 0:
                    this.WriteInteger(5);
                    this.WriteString(Name);
                    this.WriteInteger(2);
                    this.WriteString("--" + Name + "--");
                    this.WriteString("Xx" + Name + "xX");
                    break;
                default:
                    this.WriteInteger(0);
                    this.WriteString(Name);
                    this.WriteInteger(0);
                    break;
            }
        }

        private int NameAvailable(string Username)
        {
            Username = Username.ToLower();

            if (Username.Length > 15)
            {
                return -2;
            }

            if (Username.Length < 3)
            {
                return -2;
            }

            if (!WibboEnvironment.IsValidAlphaNumeric(Username))
            {
                return -1;
            }

            return WibboEnvironment.UsernameExists(Username) ? 0 : 1;
        }
    }
}
