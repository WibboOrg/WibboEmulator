using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.GameClients;
using Butterfly.Utilities;
using System;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SendRoomInviteEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
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

            if (Session.GetHabbo().IgnoreAll)
            {
                return;
            }

            ServerPacket Message = new ServerPacket(ServerPacketHeader.MESSENGER_ROOM_INVITE);
            Message.WriteInteger(Session.GetHabbo().Id);
            Message.WriteString(TextMessage);

            foreach (int UserId in Targets)
            {
                if (Session.GetHabbo().GetMessenger().FriendshipExists(UserId))
                {
                    GameClient clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                    if (clientByUserId == null)
                    {
                        break;
                    }

                    if (clientByUserId.GetHabbo().GetMessenger().FriendshipExists(Session.GetHabbo().Id))
                    {
                        clientByUserId.SendPacket(Message);
                    }
                }
            }
        }
    }
}
