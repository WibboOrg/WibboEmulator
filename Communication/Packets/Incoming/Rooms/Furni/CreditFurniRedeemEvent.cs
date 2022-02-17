using Butterfly.Communication.Packets.Outgoing.Inventory.Achievements;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class CreditFurniRedeemEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
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
                ItemDao.Delete(dbClient, Exchange.Id);
            }

            Room.GetRoomItemHandler().RemoveFurniture(null, Exchange.Id);

            int Value = int.Parse(Exchange.GetBaseItem().ItemName.Split(new char[1] { '_' })[1]);

            if (Value > 0)
            {
                if (Exchange.GetBaseItem().ItemName.StartsWith("CF_") || Exchange.GetBaseItem().ItemName.StartsWith("CFC_"))
                {
                    Session.GetHabbo().Credits += Value;
                    Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));
                }
                else if (Exchange.GetBaseItem().ItemName.StartsWith("PntEx_"))
                {
                    Session.GetHabbo().WibboPoints += Value;
                    Session.SendPacket(new ActivityPointsComposer(Session.GetHabbo().WibboPoints));

                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        UserDao.UpdateAddPoints(dbClient, Session.GetHabbo().Id, Value);
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
