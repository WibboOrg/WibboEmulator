namespace WibboEmulator.Communication.Packets.Outgoing.Handshake
{
    internal class UserRightsComposer : ServerPacket
    {
        public UserRightsComposer(int Rank)
            : base(ServerPacketHeader.USER_PERMISSIONS)
        {
            this.WriteInteger(2);//Club level
            this.WriteInteger(Rank);
            this.WriteBoolean(false);//Is an ambassador
        }
    }
}
