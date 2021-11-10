using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.GameClients;
using System;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ScrGetUserInfoMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {

            ServerPacket Message = new ServerPacket(ServerPacketHeader.USER_SUBSCRIPTION);
            Message.WriteString("habbo_club");
            double TimeLeft = 30000000;
            int TotalDaysLeft = (int)Math.Ceiling(TimeLeft / 86400);
            int MonthsLeft = TotalDaysLeft / 31;

            if (MonthsLeft >= 1)
            {
                MonthsLeft--;
            }

            Message.WriteInteger(TotalDaysLeft - (MonthsLeft * 31));
            Message.WriteInteger(2); // ??
            Message.WriteInteger(MonthsLeft);
            Message.WriteInteger(1); // type
            Message.WriteBoolean(true);
            Message.WriteBoolean(true);
            Message.WriteInteger(0);
            Message.WriteInteger(Convert.ToInt32(TimeLeft)); // days i have on hc
            Message.WriteInteger(Convert.ToInt32(TimeLeft)); // days i have on vip
            Session.SendPacket(Message);
        }
    }
}
