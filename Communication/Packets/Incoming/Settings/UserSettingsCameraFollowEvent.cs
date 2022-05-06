using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UserSettingsCameraFollowEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            bool flag = Packet.PopBoolean();

            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateCameraFollowDisabled(dbClient, Session.GetUser().Id, flag);
            }

            Session.GetUser().CameraFollowDisabled = flag;
        }
    }
}
