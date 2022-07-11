namespace WibboEmulator.Communication.Packets.Outgoing.Users
{
    internal class ScrSendUserInfoComposer : ServerPacket
    {
        public ScrSendUserInfoComposer(int timeLeft, int totalDaysLeft, int monthsLeft)
            : base(ServerPacketHeader.USER_SUBSCRIPTION)
        {
            this.WriteString("habbo_club");
            this.WriteInteger(totalDaysLeft - (monthsLeft * 31)); // display days
            this.WriteInteger(2); // ??
            this.WriteInteger(monthsLeft); // display months
            this.WriteInteger(1); // type
            this.WriteBoolean(true); // hc
            this.WriteBoolean(true); // vip
            this.WriteInteger(0); // unknow
            this.WriteInteger(timeLeft); // days i have on hc
            this.WriteInteger(timeLeft); // days i have on vip
        }
    }
}
