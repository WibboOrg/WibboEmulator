namespace WibboEmulator.Communication.Packets.Outgoing.Users
{
    internal class UpdateUsernameComposer : ServerPacket
    {
        public UpdateUsernameComposer(string Name)
            : base(ServerPacketHeader.USER_CHANGE_NAME)
        {
            this.WriteInteger(0);
            this.WriteString(Name);
            this.WriteInteger(0);
        }

        public UpdateUsernameComposer(string Name, int VirtualId)
            : base(ServerPacketHeader.USER_CHANGE_NAME)
        {
            this.WriteInteger(VirtualId);
            this.WriteString(Name);
            this.WriteInteger(1);
            this.WriteString(Name);
        }
    }
}
