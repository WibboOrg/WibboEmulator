using Butterfly.Communication.Packets.Outgoing.Messenger;
using Butterfly.Game.Clients;
using Butterfly.Utilities;
using System;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SendRoomInviteEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(Client Session, ClientPacket Packet)
        {
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

            int InviteCount = Packet.PopInt();
            if (InviteCount > 200)
            {
                return;
            }

            List<int> Targets = new List<int>();
            for (int i = 0; i < InviteCount; ++i)
            {
                int Id = Packet.PopInt();
                if (i < 100)
                {
                    Targets.Add(Id);
                }
            }

            string TextMessage = StringCharFilter.Escape(Packet.PopString());
            if (TextMessage.Length > 121)
            {
                TextMessage = TextMessage.Substring(0, 121);
            }

            if (Session.Antipub(TextMessage, "<RM>"))
            {
                return;
            }

            if (Session.GetUser().IgnoreAll)
            {
                return;
            }

            foreach (int UserId in Targets)
            {
                if (Session.GetUser().GetMessenger().FriendshipExists(UserId))
                {
                    Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                    if (clientByUserId == null || clientByUserId.GetUser().AllowConsoleMessages == false)
                    {
                        break;
                    }

                    if (clientByUserId.GetUser().GetMessenger().FriendshipExists(Session.GetUser().Id))
                    {
                        clientByUserId.SendPacket(new RoomInviteComposer(Session.GetUser().Id, TextMessage));
                    }
                }
            }
        }
    }
}
