namespace Butterfly.Communication.Packets.Outgoing.WebSocket.Troc
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
