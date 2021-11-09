using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.LandingView;
using System;
using System.Collections.Generic;
using System.Data;


namespace Butterfly.HabboHotel.LandingView
{
    public class LandingViewManager
    {
        public List <SmallPromo> HotelViewPromosIndexers = new List<SmallPromo>();

        public LandingViewManager()
        {
            this.InitHotelViewPromo();
        }

        public void InitHotelViewPromo()
        {
            this.HotelViewPromosIndexers.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable dTable = EmulatorHotelviewPromoDao.GetAll(dbClient);

                foreach (DataRow dRow in dTable.Rows)
                {
                    this.HotelViewPromosIndexers.Add(new SmallPromo(Convert.ToInt32(dRow[0]), (string)dRow[1], (string)dRow[2], (string)dRow[3], Convert.ToInt32(dRow[4]), (string)dRow[5], (string)dRow[6]));
                }
            }
        }

        public ServerPacket SmallPromoComposer(ServerPacket Message)
        {
            Message.WriteInteger(this.HotelViewPromosIndexers.Count);
            foreach (SmallPromo promo in this.HotelViewPromosIndexers)
            {
                promo.Serialize(Message);
            }

            return Message;
        }

    }
}
