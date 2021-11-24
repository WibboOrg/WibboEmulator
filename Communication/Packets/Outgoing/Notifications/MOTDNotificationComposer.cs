namespace Butterfly.Communication.Packets.Outgoing.Notifications
{
    internal class MOTDNotificationComposer : ServerPacket
    {
        public MOTDNotificationComposer(string Message)
            : base(ServerPacketHeader.GENERIC_ALERT_MESSAGES)
        {
            WriteInteger(1);
            WriteString(Message);

        }
    }
}
