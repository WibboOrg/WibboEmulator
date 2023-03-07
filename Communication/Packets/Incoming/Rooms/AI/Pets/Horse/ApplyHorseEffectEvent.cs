namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets.Horse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class ApplyHorseEffectEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.InRoom)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        var itemId = packet.PopInt();
        var item = room.RoomItemHandling.GetItem(itemId);
        if (item == null)
        {
            return;
        }

        var petId = packet.PopInt();

        if (!room.RoomUserManager.TryGetPet(petId, out var petUser))
        {
            return;
        }

        if (petUser.PetData == null || petUser.PetData.OwnerId != session.User.Id || petUser.PetData.Type != 13)
        {
            return;
        }

        if (item.Data.InteractionType == InteractionType.HORSE_SADDLE_1)
        {
            petUser.PetData.Saddle = 9;
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.UpdateHaveSaddle(dbClient, petUser.PetData.PetId, 1);
                ItemDao.DeleteById(dbClient, item.Id);
            }

            room.RoomItemHandling.RemoveFurniture(session, item.Id);
        }
        else if (item.Data.InteractionType == InteractionType.HORSE_SADDLE_2)
        {
            petUser.PetData.Saddle = 10;
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.UpdateHaveSaddle(dbClient, petUser.PetData.PetId, 2);
                ItemDao.DeleteById(dbClient, item.Id);
            }

            room.RoomItemHandling.RemoveFurniture(session, item.Id);
        }
        else if (item.Data.InteractionType == InteractionType.HORSE_HAIRSTYLE)
        {
            var parse = 100;
            var hairType = item.GetBaseItem().ItemName.Split('_')[2];

            parse += int.Parse(hairType);

            petUser.PetData.PetHair = parse;
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.UpdatePethair(dbClient, petUser.PetData.PetId, petUser.PetData.PetHair);
                ItemDao.DeleteById(dbClient, item.Id);
            }

            room.RoomItemHandling.RemoveFurniture(session, item.Id);
        }
        else if (item.Data.InteractionType == InteractionType.HORSE_HAIR_DYE)
        {
            var hairDye = 48;
            var hairType = item.GetBaseItem().ItemName.Split('_')[2];

            hairDye += int.Parse(hairType);
            petUser.PetData.HairDye = hairDye;

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.UpdateHairdye(dbClient, petUser.PetData.PetId, petUser.PetData.HairDye);
                ItemDao.DeleteById(dbClient, item.Id);
            }

            room.RoomItemHandling.RemoveFurniture(session, item.Id);
        }
        else if (item.Data.InteractionType == InteractionType.HORSE_BODY_DYE)
        {
            var race = item.GetBaseItem().ItemName.Split('_')[2];
            var parse = int.Parse(race);
            var raceLast = 2 + (parse * 4) - 4;
            if (parse == 13)
            {
                raceLast = 61;
            }
            else if (parse == 14)
            {
                raceLast = 65;
            }
            else if (parse == 15)
            {
                raceLast = 69;
            }
            else if (parse == 16)
            {
                raceLast = 73;
            }

            petUser.PetData.Race = raceLast.ToString();

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.UpdateRace(dbClient, petUser.PetData.PetId, petUser.PetData.Race);
                ItemDao.DeleteById(dbClient, item.Id);
            }

            //We only want to use this if we're successful. 
            room.RoomItemHandling.RemoveFurniture(session, item.Id);
        }

        room.SendPacket(new UsersComposer(petUser));
        room.SendPacket(new PetHorseFigureInformationComposer(petUser));
    }
}
