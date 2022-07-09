namespace Wibbo.Communication.Packets.Outgoing.Users
{
    internal class RespectNotificationComposer : ServerPacket
    {
        public RespectNotificationComposer(int Id, int Respect)
            : base(ServerPacketHeader.USER_RESPECT)
        {
            this.WriteInteger(Id);
            this.WriteInteger(Respect);
        }
    }
}
