namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class NewConsoleComposer : ServerPacket
    {
        public NewConsoleComposer(int Sender, string Message, int Time = 0)
            : base(ServerPacketHeader.MESSENGER_CHAT)
        {
            this.WriteInteger(Sender);
            this.WriteString(Message);
            this.WriteInteger(Time);
        }
    }
}
