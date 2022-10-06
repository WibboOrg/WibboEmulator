namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets.Horse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class ApplyHorseEffectEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().InRoom)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var Room))
        {
            return;
        }

        var ItemId = packet.PopInt();
        var Item = Room.GetRoomItemHandler().GetItem(ItemId);
        if (Item == null)
        {
            return;
        }

        var PetId = packet.PopInt();

        if (!Room.GetRoomUserManager().TryGetPet(PetId, out var PetUser))
        {
            return;
        }

        if (PetUser.PetData == null || PetUser.PetData.OwnerId != session.GetUser().Id || PetUser.PetData.Type != 13)
        {
            return;
        }

        if (Item.Data.InteractionType == InteractionType.HORSE_SADDLE_1)
        {
            PetUser.PetData.Saddle = 9;
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.UpdateHaveSaddle(dbClient, PetUser.PetData.PetId, 1);
                ItemDao.Delete(dbClient, Item.Id);
            }

            //We only want to use this if we're successful. 
            Room.GetRoomItemHandler().RemoveFurniture(session, Item.Id);
        }
        else if (Item.Data.InteractionType == InteractionType.HORSE_SADDLE_2)
        {
            PetUser.PetData.Saddle = 10;
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.UpdateHaveSaddle(dbClient, PetUser.PetData.PetId, 2);
                ItemDao.Delete(dbClient, Item.Id);
            }

            //We only want to use this if we're successful. 
            Room.GetRoomItemHandler().RemoveFurniture(session, Item.Id);
        }
        else if (Item.Data.InteractionType == InteractionType.HORSE_HAIRSTYLE)
        {
            var Parse = 100;
            var HairType = Item.GetBaseItem().ItemName.Split('_')[2];

            Parse += int.Parse(HairType);

            PetUser.PetData.PetHair = Parse;
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.UpdatePethair(dbClient, PetUser.PetData.PetId, PetUser.PetData.PetHair);
                ItemDao.Delete(dbClient, Item.Id);
            }

            //We only want to use this if we're successful. 
            Room.GetRoomItemHandler().RemoveFurniture(session, Item.Id);
        }
        else if (Item.Data.InteractionType == InteractionType.HORSE_HAIR_DYE)
        {
            var HairDye = 48;
            var HairType = Item.GetBaseItem().ItemName.Split('_')[2];

            HairDye += int.Parse(HairType);
            PetUser.PetData.HairDye = HairDye;

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.UpdateHairdye(dbClient, PetUser.PetData.PetId, PetUser.PetData.HairDye);
                ItemDao.Delete(dbClient, Item.Id);
            }

            //We only want to use this if we're successful. 
            Room.GetRoomItemHandler().RemoveFurniture(session, Item.Id);
        }
        else if (Item.Data.InteractionType == InteractionType.HORSE_BODY_DYE)
        {
            var Race = Item.GetBaseItem().ItemName.Split('_')[2];
            var Parse = int.Parse(Race);
            var RaceLast = 2 + (Parse * 4) - 4;
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

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.UpdateRace(dbClient, PetUser.PetData.PetId, PetUser.PetData.Race);
                ItemDao.Delete(dbClient, Item.Id);
            }

            //We only want to use this if we're successful. 
            Room.GetRoomItemHandler().RemoveFurniture(session, Item.Id);
        }

        Room.SendPacket(new UsersComposer(PetUser));
        Room.SendPacket(new PetHorseFigureInformationComposer(PetUser));
    }
}