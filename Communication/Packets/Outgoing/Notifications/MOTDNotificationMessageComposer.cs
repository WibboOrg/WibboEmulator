namespace Butterfly.Communication.Packets.Outgoing.Notifications
{
    internal class MOTDNotificationMessageComposer : ServerPacket
    {
        public MOTDNotificationMessageComposer(string Message)
            : base(ServerPacketHeader.GENERIC_ALERT_MESSAGES)
        {
            WriteInteger(1);
            WriteString(Message);

        }
    }
}
