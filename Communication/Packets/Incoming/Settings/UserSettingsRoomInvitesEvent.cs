using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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
