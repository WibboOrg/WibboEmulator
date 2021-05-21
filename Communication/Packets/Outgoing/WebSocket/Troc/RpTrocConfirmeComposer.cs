namespace Butterfly.Communication.Packets.Outgoing.WebSocket.Troc
{
    internal class RpTrocConfirmeComposer : ServerPacket
    {
        public RpTrocConfirmeComposer(int UserId)
          : base(16)
        {
            this.WriteInteger(UserId);
        }
    }
}
