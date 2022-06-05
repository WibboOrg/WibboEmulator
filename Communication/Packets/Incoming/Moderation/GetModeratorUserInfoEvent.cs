using Wibbo.Communication.Packets.Outgoing.Moderation;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using System.Data;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorUserInfoEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasFuse("fuse_mod"))
            {
                return;
            }

            int userId = Packet.PopInt();
            if (WibboEnvironment.GetGame().GetClientManager().GetNameById(userId) != "")
            {
                Client client = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
                DataRow user = null;
                DataRow info = null;

                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    user = UserDao.GetOneInfo(dbClient, userId);
                }

                if (user == null)
                {
                    return;
                }

                Session.SendPacket(new ModeratorUserInfoComposer(user, info));
            }
            else
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.loadusererror", Session.Langue));
            }
        }
    }
}
