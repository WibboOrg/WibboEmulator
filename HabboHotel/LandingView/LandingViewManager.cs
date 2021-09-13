using Butterfly.Communication.Packets.Outgoing;
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

            using (IQueryAdapter DbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("SELECT * from hotelview_promos WHERE hotelview_promos.enabled = '1' ORDER BY hotelview_promos.`index` ASC");
                DataTable dTable = DbClient.GetTable();

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
