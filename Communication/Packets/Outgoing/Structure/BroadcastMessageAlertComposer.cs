namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class BroadcastMessageAlertComposer : ServerPacket
    {
        public BroadcastMessageAlertComposer(string Message, string URL = "")
            : base(ServerPacketHeader.GENERIC_ALERT)
        {
            this.WriteString(Message);
            this.WriteString(URL);
        }
    }
}
