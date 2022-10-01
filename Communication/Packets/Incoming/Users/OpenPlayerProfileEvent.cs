using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Users;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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
                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                friendCount = MessengerFriendshipDao.GetCount(dbClient, userId);
            }

            Session.SendPacket(new ProfileInformationComposer(targetData, Session, Groups, friendCount));
        }
    }
}