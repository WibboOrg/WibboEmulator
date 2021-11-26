namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class HelperToolComposer : ServerPacket
    {
        public HelperToolComposer(bool onDuty, int count)
            : base(ServerPacketHeader.HelperToolMessageComposer)
        {
            WriteBoolean(onDuty);
            WriteInteger(count);
            WriteInteger(0);
            WriteInteger(0);
        }
    }
}
