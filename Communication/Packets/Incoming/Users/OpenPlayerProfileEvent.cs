using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Guilds;
using Butterfly.Game.Users;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class OpenPlayerProfileEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int userId = Packet.PopInt();
            bool IsMe = Packet.PopBoolean();

            User targetData = ButterflyEnvironment.GetUserById(userId);
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
                    friendCount = MessengerFriendshipDao.GetCount(dbClient, userId);
                }
            }

            Session.SendPacket(new ProfileInformationComposer(targetData, Session, Groups, friendCount));
        }
    }
}