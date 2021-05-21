using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Users;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class OpenPlayerProfileEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int userID = Packet.PopInt();
            bool IsMe = Packet.PopBoolean();

            Habbo targetData = ButterflyEnvironment.GetHabboById(userID);
            if (targetData == null)
            {
                return;
            }

            List<Group> Groups = ButterflyEnvironment.GetGame().GetGroupManager().GetGroupsForUser(targetData.MyGroups);

            int friendCount = 0;

            if (targetData.GetMessenger() != null)
            {
                friendCount = targetData.GetMessenger().Count;
            }
            else
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT COUNT(0) FROM `messenger_friendships` WHERE (`user_one_id` = @userid);");
                    dbClient.AddParameter("userid", userID);
                    friendCount = dbClient.GetInteger();
                }
            }

            Session.SendPacket(new ProfileInformationComposer(targetData, Session, Groups, friendCount));
        }
    }
}