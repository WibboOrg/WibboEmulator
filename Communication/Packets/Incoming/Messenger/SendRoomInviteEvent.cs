using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class SendRoomInviteEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(GameClient Session, ClientPacket Packet)
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

            int inviteCount = Packet.PopInt();
            if (inviteCount > 100)
            {
                return;
            }

            List<int> targets = new List<int>();
            for (int i = 0; i < inviteCount; ++i)
            {
                int Id = Packet.PopInt();
                if (i < 100)
                {
                    targets.Add(Id);
                }
            }

            string textMessage = StringCharFilter.Escape(Packet.PopString());
            if (textMessage.Length > 121)
            {
                textMessage = textMessage.Substring(0, 121);
            }

            if (Session.Antipub(textMessage, "<RM>"))
            {
                return;
            }

            if (Session.GetUser().IgnoreAll)
            {
                return;
            }


            ServerPacket roomInvitePacket = new RoomInviteComposer(Session.GetUser().Id, textMessage);

            foreach (int UserId in targets)
            {
                if (Session.GetUser().GetMessenger().FriendshipExists(UserId))
                {
                    GameClient clientByUserId = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                    if (clientByUserId == null || clientByUserId.GetUser().IgnoreRoomInvites)
                    {
                        break;
                    }

                    if (clientByUserId.GetUser().GetMessenger().FriendshipExists(Session.GetUser().Id))
                    {
                        clientByUserId.SendPacket(roomInvitePacket);
                    }
                }
            }
        }
    }
}
