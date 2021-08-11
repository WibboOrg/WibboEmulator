namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class ScrSendUserInfoComposer : ServerPacket
    {
        public ScrSendUserInfoComposer()
            : base(ServerPacketHeader.USER_SUBSCRIPTION)
        {
            int DisplayMonths = 0;
            int DisplayDays = 0;

            this.WriteString("habbo_club");
            this.WriteInteger(DisplayDays);
            this.WriteInteger(2);
            this.WriteInteger(DisplayMonths);
            this.WriteInteger(1);
            this.WriteBoolean(true); // hc
            this.WriteBoolean(true); // vip
            this.WriteInteger(0);
            this.WriteInteger(0);
            this.WriteInteger(495);
        }
    }
}
