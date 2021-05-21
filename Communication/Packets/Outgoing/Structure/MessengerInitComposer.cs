namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class MessengerInitComposer : ServerPacket
    {
        public MessengerInitComposer()
            : base(ServerPacketHeader.MESSENGER_INIT)
        {
            this.WriteInteger(3000);
            this.WriteInteger(300);
            this.WriteInteger(800);
            this.WriteInteger(0);
        }
    }
}
