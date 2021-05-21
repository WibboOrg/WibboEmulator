namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class AddChatlogsComposer : ServerPacket
    {
        public AddChatlogsComposer(int UserId, string Pseudo, string Message)
          : base(7)
        {
            this.WriteInteger(UserId);
            this.WriteString(Pseudo);
            this.WriteString(Message);
        }
    }
}