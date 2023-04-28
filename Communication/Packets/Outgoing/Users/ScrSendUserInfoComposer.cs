namespace WibboEmulator.Communication.Packets.Outgoing.Users;

internal sealed class ScrSendUserInfoComposer : ServerPacket
{
    public ScrSendUserInfoComposer(TimeSpan expireTime, bool isLogin = true, bool hasEverBeenMember = false)
        : base(ServerPacketHeader.USER_SUBSCRIPTION)
    {
        this.WriteString("habbo_club"); //productName
        this.WriteInteger(expireTime.TotalSeconds > 0 ? (int)expireTime.TotalDays + 1 : 0); //daysToPeriodEnd
        this.WriteInteger(0); //memberPeriods
        this.WriteInteger(0); //periodsSubscribedAhead
        this.WriteInteger(isLogin ? 1 : 2); //responseType
        this.WriteBoolean(hasEverBeenMember); //hasEverBeenMember
        this.WriteBoolean(expireTime.TotalSeconds > 0); //isVip
        this.WriteInteger(0); //pastClubDays
        this.WriteInteger(0); //pastVipDays
        this.WriteInteger((int)expireTime.TotalMinutes); //minutesUntilExpiration
    }
}
