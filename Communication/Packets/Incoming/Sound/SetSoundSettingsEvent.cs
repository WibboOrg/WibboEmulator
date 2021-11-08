using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SetSoundSettingsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            int Volume1 = Packet.PopInt();
            int Volume2 = Packet.PopInt();
            int Volume3 = Packet.PopInt();


            if (Session.GetHabbo().ClientVolume[0] == Volume1 && Session.GetHabbo().ClientVolume[1] == Volume2 && Session.GetHabbo().ClientVolume[2] == Volume3)
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateVolume(dbClient, Session.GetHabbo().Id, Volume1, + Volume2, + Volume3);
            }

            Session.GetHabbo().ClientVolume.Clear();
            Session.GetHabbo().ClientVolume.Add(Volume1);
            Session.GetHabbo().ClientVolume.Add(Volume2);
            Session.GetHabbo().ClientVolume.Add(Volume3);

            Session.GetHabbo().SendWebPacket(new SettingVolumeComposer(Volume3, Volume2, Volume1));
        }
    }
}
