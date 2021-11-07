using Butterfly.Communication.Packets.Outgoing.Inventory.Achievements;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class CreditFurniRedeemEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                return;
            }

            Item Exchange = Room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (Exchange == null)
            {
                return;
            }

            if (Exchange.Data.InteractionType != InteractionType.EXCHANGE)
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.id = '" + Exchange.Id + "'");
            }

            Room.GetRoomItemHandler().RemoveFurniture(null, Exchange.Id);

            int Value = int.Parse(Exchange.GetBaseItem().ItemName.Split(new char[1] { '_' })[1]);

            if (Value > 0)
            {
                if (Exchange.GetBaseItem().ItemName.StartsWith("CF_") || Exchange.GetBaseItem().ItemName.StartsWith("CFC_"))
                {
                    Session.GetHabbo().Credits += Value;
                    Session.GetHabbo().UpdateCreditsBalance();
                }
                else if (Exchange.GetBaseItem().ItemName.StartsWith("PntEx_"))
                {
                    Session.GetHabbo().WibboPoints += Value;
                    Session.SendPacket(new ActivityPointsComposer(Session.GetHabbo().WibboPoints));

                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE users SET vip_points = vip_points + '" + Value + "' WHERE id = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }
                }
                else if (Exchange.GetBaseItem().ItemName.StartsWith("WwnEx_"))
                {
                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        UserStatsDao.UpdateAchievementScore(dbClient, Session.GetHabbo().Id, Value);
                    }

                    Session.GetHabbo().AchievementPoints += Value;
                    Session.SendPacket(new AchievementScoreComposer(Session.GetHabbo().AchievementPoints));

                    if (Room != null)
                    {
                        RoomUser roomUserByHabbo = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                        if (roomUserByHabbo != null)
                        {
                            Session.SendPacket(new UserChangeComposer(roomUserByHabbo, true));
                            Room.SendPacket(new UserChangeComposer(roomUserByHabbo, false));
                        }
                    }
                }
            }
        }
    }
}
