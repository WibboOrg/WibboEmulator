namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class AddChatlogsComposer : ServerPacket
    {
        public AddChatlogsComposer(int UserId, string Pseudo, string Message)
          : base(ServerPacketHeader.ADD_CHATLOGS)
        {
            this.WriteInteger(UserId);
            this.WriteString(Pseudo);
            this.WriteString(Message);
        }
    }
}