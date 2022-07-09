using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class UserSettingsRoomInvitesEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            bool flag = Packet.PopBoolean();

            if(Session == null || Session.GetUser() == null)
            {
                return;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateIgnoreRoomInvites(dbClient, Session.GetUser().Id, flag);
            }

            Session.GetUser().IgnoreRoomInvites = flag;
        }
    }
}
