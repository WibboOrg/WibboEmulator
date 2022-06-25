namespace WibboEmulator.Communication.Packets.Outgoing.Notifications
{
    internal class InClientLinkComposer : ServerPacket
    {
        public InClientLinkComposer(string Message)
            : base(ServerPacketHeader.IN_CLIENT_LINK)
        {
            this.WriteString(Message);
        }

    }
}
