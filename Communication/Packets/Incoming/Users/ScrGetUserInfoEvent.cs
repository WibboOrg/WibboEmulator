namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;

internal class ScrGetUserInfoMessageEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {

        double timeLeft = 30000000;
        var totalDaysLeft = (int)Math.Ceiling(timeLeft / 86400);
        var monthsLeft = totalDaysLeft / 31;

        if (monthsLeft >= 1)
        {
            monthsLeft--;
        }

        session.SendPacket(new ScrSendUserInfoComposer(Convert.ToInt32(timeLeft), totalDaysLeft, monthsLeft));
    }
}
