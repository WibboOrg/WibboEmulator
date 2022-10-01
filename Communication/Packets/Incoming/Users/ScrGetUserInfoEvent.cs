using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class ScrGetUserInfoMessageEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {

            double timeLeft = 30000000;
            int totalDaysLeft = (int)Math.Ceiling(timeLeft / 86400);
            int monthsLeft = totalDaysLeft / 31;

            if (monthsLeft >= 1)
            {
                monthsLeft--;
            }

            Session.SendPacket(new ScrSendUserInfoComposer(Convert.ToInt32(timeLeft), totalDaysLeft, monthsLeft));
        }
    }
}
