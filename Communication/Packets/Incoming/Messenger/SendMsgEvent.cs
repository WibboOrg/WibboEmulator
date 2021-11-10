using Butterfly.Game.GameClients;
using System;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SendMsgEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            int userId = Packet.PopInt();

            if (userId == Session.GetHabbo().Id)
            {
                return;
            }

            string Message = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
            if (string.IsNullOrWhiteSpace(Message))
            {
                return;
            }

            TimeSpan timeSpan = DateTime.Now - Session.GetHabbo().FloodTime;
            if (timeSpan.Seconds > 4)
            {
                Session.GetHabbo().FloodCount = 0;
            }

            if (timeSpan.Seconds < 4 && Session.GetHabbo().FloodCount > 5 && Session.GetHabbo().Rank < 5)
            {
                return;
            }

            Session.GetHabbo().FloodTime = DateTime.Now;
            Session.GetHabbo().FloodCount++;

            if (Session.Antipub("<" + userId + "> " + Message, "<MP>"))
            {
                return;
            }

            if (Session.GetHabbo().IgnoreAll)
            {
                return;
            }

            Session.GetHabbo().GetMessenger().SendInstantMessage(userId, Message);
        }
    }
}