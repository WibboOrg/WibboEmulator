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
            if (!Session.GetUser().InRoom)
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room Room))
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
                    Session.GetUser().Credits += Value;
                    Session.SendPacket(new CreditBalanceComposer(Session.GetUser().Credits));
                }
                else if (Exchange.GetBaseItem().ItemName.StartsWith("PntEx_"))
                {
                    Session.GetUser().WibboPoints += Value;
                    Session.SendPacket(new ActivityPointsComposer(Session.GetUser().WibboPoints));

                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        UserDao.UpdateAddPoints(dbClient, Session.GetUser().Id, Value);
                    }
                }
                else if (Exchange.GetBaseItem().ItemName.StartsWith("WwnEx_"))
                {
                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        UserStatsDao.UpdateAchievementScore(dbClient, Session.GetUser().Id, Value);
                    }

                    Session.GetUser().AchievementPoints += Value;
                    Session.SendPacket(new AchievementScoreComposer(Session.GetUser().AchievementPoints));

                    if (Room != null)
                    {
                        RoomUser roomUserByUserId = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
                        if (roomUserByUserId != null)
                        {
                            Session.SendPacket(new UserChangeComposer(roomUserByUserId, true));
                            Room.SendPacket(new UserChangeComposer(roomUserByUserId, false));
                        }
                    }
                }
            }
        }
    }
}
