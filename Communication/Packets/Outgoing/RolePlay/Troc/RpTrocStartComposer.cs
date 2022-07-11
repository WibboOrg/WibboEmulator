namespace WibboEmulator.Communication.Packets.Outgoing.RolePlay.Troc
{
    internal class RpTrocStartComposer : ServerPacket
    {
        public RpTrocStartComposer(int UserId, string Username)
          : base(ServerPacketHeader.RP_TROC_START)
        {
            this.WriteInteger(UserId);
            this.WriteString(Username);
        }
    }
}
