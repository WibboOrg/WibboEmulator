namespace Butterfly.Communication.Packets.Outgoing.Notifications
{
    internal class NuxAlertComposer : ServerPacket
    {
        public NuxAlertComposer(string Message)
            : base(ServerPacketHeader.IN_CLIENT_LINK)
        {
            this.WriteString(Message);
        }

    }
}
