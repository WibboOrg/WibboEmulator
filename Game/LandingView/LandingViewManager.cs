using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;


namespace Butterfly.Game.LandingView
{
    public class LandingViewManager
    {
        private readonly List<SmallPromo> _hotelViewPromosIndexers;

        public LandingViewManager()
        {
            this._hotelViewPromosIndexers = new List<SmallPromo>();
        }

        public void Init()
        {
            this._hotelViewPromosIndexers.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable dTable = EmulatorHotelviewPromoDao.GetAll(dbClient);

                foreach (DataRow dRow in dTable.Rows)
                {
                    this._hotelViewPromosIndexers.Add(new SmallPromo(Convert.ToInt32(dRow[0]), (string)dRow[1], (string)dRow[2], (string)dRow[3], Convert.ToInt32(dRow[4]), (string)dRow[5], (string)dRow[6]));
                }
            }
        }

        public ServerPacket SmallPromoComposer(ServerPacket Message)
        {
            Message.WriteInteger(this._hotelViewPromosIndexers.Count);
            foreach (SmallPromo promo in this._hotelViewPromosIndexers)
            {
                promo.Serialize(Message);
            }

            return Message;
        }

        public int Count()
        {
            return this._hotelViewPromosIndexers.Count;
        }

    }
}
