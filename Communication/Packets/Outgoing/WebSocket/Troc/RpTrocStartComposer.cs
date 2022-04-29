namespace Butterfly.Communication.Packets.Outgoing.WebSocket.Troc
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
