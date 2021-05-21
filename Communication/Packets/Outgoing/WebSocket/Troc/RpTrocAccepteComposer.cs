namespace Butterfly.Communication.Packets.Outgoing.WebSocket.Troc
{
    internal class RpTrocAccepteComposer : ServerPacket
    {
        public RpTrocAccepteComposer(int UserId, bool Etat)
          : base(15)
        {
            this.WriteInteger(UserId);
            this.WriteBoolean(Etat);
        }
    }
}
