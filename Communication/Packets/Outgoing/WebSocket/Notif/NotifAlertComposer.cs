namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class NotifAlertComposer : ServerPacket
    {
        public NotifAlertComposer(string Image, string Title, string Message, string TextButton, int RoomId, string Url)
         : base(12)
        {
            this.WriteString(Image);
            this.WriteString(Title);
            this.WriteString(Message);
            this.WriteString(TextButton);
            this.WriteInteger(RoomId);
            this.WriteString(Url);
        }
    }
}
