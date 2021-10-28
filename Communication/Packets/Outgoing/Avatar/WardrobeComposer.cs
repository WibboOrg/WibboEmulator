using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using System;
using System.Data;

namespace Butterfly.Communication.Packets.Outgoing.Avatar
{
    internal class WardrobeComposer : ServerPacket
    {
        public WardrobeComposer(GameClient Session)
            : base(ServerPacketHeader.USER_OUTFITS)
        {
            this.WriteInteger(1);
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT slot_id,look,gender FROM user_wardrobe WHERE user_id = '" + Session.GetHabbo().Id + "' LIMIT 24");
                DataTable WardrobeData = dbClient.GetTable();

                if (WardrobeData == null)
                {
                    this.WriteInteger(0);
                }
                else
                {
                    this.WriteInteger(WardrobeData.Rows.Count);
                    foreach (DataRow Row in WardrobeData.Rows)
                    {
                        this.WriteInteger(Convert.ToInt32(Row["slot_id"]));
                        this.WriteString(Convert.ToString(Row["look"]));
                        this.WriteString(Row["gender"].ToString().ToUpper());
                    }
                }
            }
        }
    }
}
