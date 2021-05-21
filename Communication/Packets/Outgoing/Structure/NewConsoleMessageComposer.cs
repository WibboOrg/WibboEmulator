namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class NewConsoleMessageComposer : ServerPacket
    {
        public NewConsoleMessageComposer(int Sender, string Message, int Time = 0)
            : base(ServerPacketHeader.MESSENGER_CHAT)
        {
            this.WriteInteger(Sender);
            this.WriteString(Message);
            this.WriteInteger(Time);
        }
    }
}
