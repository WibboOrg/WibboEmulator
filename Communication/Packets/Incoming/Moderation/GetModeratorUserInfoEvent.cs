using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using System.Data;

namespace Butterfly.Communication.Packets.Incoming.Structure
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
            if (ButterflyEnvironment.GetGame().GetClientManager().GetNameById(userId) != "")
            {
                Client client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
                DataRow user = null;
                DataRow info = null;

                if (client == null)
                {
                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        user = UserDao.GetOneIdAndName(dbClient, userId);
                    }
                    if (user == null)
                    {
                        return;
                    }
                }

                Session.SendPacket(new ModeratorUserInfoComposer(user, info));
            }
            else
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("user.loadusererror", Session.Langue));
            }
        }
    }
}
