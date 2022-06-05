namespace Wibbo.Communication.Packets.Outgoing.RolePlay.Troc
{
    internal class RpTrocAccepteComposer : ServerPacket
    {
        public RpTrocAccepteComposer(int UserId, bool Etat)
          : base(ServerPacketHeader.RP_TROC_ACCEPTE)
        {
            this.WriteInteger(UserId);
            this.WriteBoolean(Etat);
        }
    }
}
