using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ApplyHorseEffectEvent : IPacketEvent
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

            int ItemId = Packet.PopInt();
            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
            {
                return;
            }

            int PetId = Packet.PopInt();

            if (!Room.GetRoomUserManager().TryGetPet(PetId, out RoomUser PetUser))
            {
                return;
            }

            if (PetUser.PetData == null || PetUser.PetData.OwnerId != Session.GetHabbo().Id || PetUser.PetData.Type != 13)
            {
                return;
            }

            if (Item.Data.InteractionType == InteractionType.HORSE_SADDLE_1)
            {
                PetUser.PetData.Saddle = 9;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    BotPetDao.UpdateHaveSaddle(dbClient, PetUser.PetData.PetId, 1);
                    ItemDao.Delete(dbClient, Item.Id);
                }

                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_SADDLE_2)
            {
                PetUser.PetData.Saddle = 10;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    BotPetDao.UpdateHaveSaddle(dbClient, PetUser.PetData.PetId, 2);
                    ItemDao.Delete(dbClient, Item.Id);
                }

                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_HAIRSTYLE)
            {
                int Parse = 100;
                string HairType = Item.GetBaseItem().ItemName.Split('_')[2];

                Parse += int.Parse(HairType);

                PetUser.PetData.PetHair = Parse;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    BotPetDao.UpdatePethair(dbClient, PetUser.PetData.PetId, PetUser.PetData.PetHair);
                    ItemDao.Delete(dbClient, Item.Id);
                }

                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_HAIR_DYE)
            {
                int HairDye = 48;
                string HairType = Item.GetBaseItem().ItemName.Split('_')[2];

                HairDye += int.Parse(HairType);
                PetUser.PetData.HairDye = HairDye;

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    BotPetDao.UpdateHairdye(dbClient, PetUser.PetData.PetId, PetUser.PetData.HairDye);
                    ItemDao.Delete(dbClient, Item.Id);
                }

                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_BODY_DYE)
            {
                string Race = Item.GetBaseItem().ItemName.Split('_')[2];
                int Parse = int.Parse(Race);
                int RaceLast = 2 + (Parse * 4) - 4;
                if (Parse == 13)
                {
                    RaceLast = 61;
                }
                else if (Parse == 14)
                {
                    RaceLast = 65;
                }
                else if (Parse == 15)
                {
                    RaceLast = 69;
                }
                else if (Parse == 16)
                {
                    RaceLast = 73;
                }

                PetUser.PetData.Race = RaceLast.ToString();

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    BotPetDao.UpdateRace(dbClient, PetUser.PetData.PetId, PetUser.PetData.Race);
                    ItemDao.Delete(dbClient, Item.Id);
                }

                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
            }

            Room.SendPacket(new UsersComposer(PetUser));
            Room.SendPacket(new PetHorseFigureInformationComposer(PetUser));
        }
    }
}