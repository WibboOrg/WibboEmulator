using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SetSoundSettingsEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            int Volume1 = Packet.PopInt();
            int Volume2 = Packet.PopInt();
            int Volume3 = Packet.PopInt();


            if (Session.GetUser().ClientVolume[0] == Volume1 && Session.GetUser().ClientVolume[1] == Volume2 && Session.GetUser().ClientVolume[2] == Volume3)
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateVolume(dbClient, Session.GetUser().Id, Volume1, +Volume2, +Volume3);
            }

            Session.GetUser().ClientVolume.Clear();
            Session.GetUser().ClientVolume.Add(Volume1);
            Session.GetUser().ClientVolume.Add(Volume2);
            Session.GetUser().ClientVolume.Add(Volume3);
        }
    }
}
