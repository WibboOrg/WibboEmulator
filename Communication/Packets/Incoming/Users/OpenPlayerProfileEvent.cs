using Wibbo.Communication.Packets.Outgoing.Users;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Groups;
using Wibbo.Game.Users;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class OpenPlayerProfileEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int userId = Packet.PopInt();
            bool IsMe = Packet.PopBoolean();

            User targetData = WibboEnvironment.GetUserById(userId);
            if (targetData == null)
            {
                return;
            }

            List<Group> Groups = WibboEnvironment.GetGame().GetGroupManager().GetGroupsForUser(targetData.MyGroups);

            int friendCount = 0;

            if (targetData.GetMessenger() != null)
            {
                friendCount = targetData.GetMessenger().Count;
            }
            else
            {
                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    friendCount = MessengerFriendshipDao.GetCount(dbClient, userId);
                }
            }

            Session.SendPacket(new ProfileInformationComposer(targetData, Session, Groups, friendCount));
        }
    }
}