using Butterfly.Game.Clients;
using System;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SendMsgEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null || Session.GetUser().GetMessenger() == null)
            {
                return;
            }

            int userId = Packet.PopInt();

            if (userId == Session.GetUser().Id)
            {
                return;
            }

            string Message = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
            if (string.IsNullOrWhiteSpace(Message))
            {
                return;
            }

            TimeSpan timeSpan = DateTime.Now - Session.GetUser().FloodTime;
            if (timeSpan.Seconds > 4)
            {
                Session.GetUser().FloodCount = 0;
            }

            if (timeSpan.Seconds < 4 && Session.GetUser().FloodCount > 5 && Session.GetUser().Rank < 5)
            {
                return;
            }

            Session.GetUser().FloodTime = DateTime.Now;
            Session.GetUser().FloodCount++;

            if (Session.Antipub("<" + userId + "> " + Message, "<MP>"))
            {
                return;
            }

            if (Session.GetUser().IgnoreAll)
            {
                return;
            }

            Session.GetUser().GetMessenger().SendInstantMessage(userId, Message);
        }
    }
}