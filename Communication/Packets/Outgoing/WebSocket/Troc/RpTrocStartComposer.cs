namespace Butterfly.Communication.Packets.Outgoing.WebSocket.Troc
{
    internal class RpTrocStartComposer : ServerPacket
    {
        public RpTrocStartComposer(int UserId, string Username)
          : base(13)
        {
            this.WriteInteger(UserId);
            this.WriteString(Username);
        }
    }
}
