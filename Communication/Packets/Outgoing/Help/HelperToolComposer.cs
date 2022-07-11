namespace WibboEmulator.Communication.Packets.Outgoing.Help
{
    internal class HelperToolComposer : ServerPacket
    {
        public HelperToolComposer(bool onDuty, int count)
            : base(ServerPacketHeader.GUIDE_ON_DUTY_STATUS)
        {
            WriteBoolean(onDuty);
            WriteInteger(count);
            WriteInteger(0);
            WriteInteger(0);
        }
    }
}
