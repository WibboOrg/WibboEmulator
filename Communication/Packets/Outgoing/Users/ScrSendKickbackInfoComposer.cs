namespace WibboEmulator.Communication.Packets.Outgoing.Users;

internal sealed class ScrSendKickbackInfoComposer : ServerPacket
{
    public ScrSendKickbackInfoComposer(DateTime activated)
        : base(ServerPacketHeader.SCR_SEND_KICKBACK_INFO)
    {
        this.WriteInteger(0); //currentHcStreak
        this.WriteString($"{activated.Day}-{activated.Month}-{activated.Year}"); //firstSubscriptionDate
        this.WriteDouble(0); //kickbackPercentage
        this.WriteInteger(0); //totalCreditsMissed
        this.WriteInteger(0); //totalCreditsRewarded
        this.WriteInteger(0); //totalCreditsSpent
        this.WriteInteger(0); //creditRewardForStreakBonus
        this.WriteInteger(0); //creditRewardForMonthlySpent
        this.WriteInteger(0); //timeUntilPayday
    }
}
