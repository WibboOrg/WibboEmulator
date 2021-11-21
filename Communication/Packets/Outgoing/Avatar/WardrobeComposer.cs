using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using System;
using System.Data;

namespace Butterfly.Communication.Packets.Outgoing.Avatar
{
    internal class WardrobeComposer : ServerPacket
    {
        public WardrobeComposer(Client Session)
            : base(ServerPacketHeader.USER_OUTFITS)
        {
            this.WriteInteger(1);
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable WardrobeData = UserWardrobeDao.GetAll(dbClient, Session.GetHabbo().Id);

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
