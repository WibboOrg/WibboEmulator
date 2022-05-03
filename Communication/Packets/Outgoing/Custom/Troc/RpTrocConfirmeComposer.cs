namespace Butterfly.Communication.Packets.Outgoing.Custom.Troc
{
    internal class RpTrocConfirmeComposer : ServerPacket
    {
        public RpTrocConfirmeComposer(int UserId)
          : base(ServerPacketHeader.RP_TROC_CONFIRME)
        {
            this.WriteInteger(UserId);
        }
    }
}
