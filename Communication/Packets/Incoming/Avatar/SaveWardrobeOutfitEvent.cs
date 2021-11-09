using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SaveWardrobeOutfitEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int slotId = Packet.PopInt();
            string look = Packet.PopString();
            string gender = Packet.PopString();

            if (slotId < 1 || slotId > 24)
            {
                return;
            }

            if (gender != "M" && gender != "F")
            {
                return;
            }

            look = ButterflyEnvironment.GetFigureManager().ProcessFigure(look, gender, true);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                UserWardrobeDao.Insert(dbClient, Session.GetHabbo().Id, slotId, look, gender.ToUpper());
        }
    }
}
