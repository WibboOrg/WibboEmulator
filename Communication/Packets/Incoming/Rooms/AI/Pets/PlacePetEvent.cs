namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.AI;

internal class PlacePetEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            //session.SendPacket(new RoomErrorNotifComposer(1));
            return;
        }

        if (room.GetRoomUserManager().BotPetCount >= 30)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.placepet.error", session.Langue));
            return;
        }

        if (!session.GetUser().GetInventoryComponent().TryGetPet(packet.PopInt(), out var pet))
        {
            return;
        }

        if (pet == null)
        {
            return;
        }

        if (pet.PlacedInRoom)
        {
            return;
        }

        var x = packet.PopInt();
        var y = packet.PopInt();

        if (!room.GetGameMap().CanWalk(x, y, false))
        {
            return;
        }

        if (room.GetRoomUserManager().TryGetPet(pet.PetId, out var oldPet))
        {
            room.GetRoomUserManager().RemoveBot(oldPet.VirtualId, false);
        }

        pet.X = x;
        pet.Y = y;

        pet.PlacedInRoom = true;
        pet.RoomId = room.Id;

        _ = room.GetRoomUserManager().DeployBot(new RoomBot(pet.PetId, pet.OwnerId, pet.RoomId, BotAIType.Pet, true, pet.Name, "", "", pet.Look, x, y, 0, 0, false, "", 0, false, 0, 0, 0), pet);

        pet.DBState = DatabaseUpdateState.NEEDS_UPDATE;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            BotPetDao.UpdateRoomId(dbClient, pet.PetId, pet.RoomId);
        }

        if (!session.GetUser().GetInventoryComponent().TryRemovePet(pet.PetId, out var toRemove))
        {
            Console.WriteLine("Error whilst removing pet: " + toRemove.PetId);
            return;
        }

        session.SendPacket(new PetInventoryComposer(session.GetUser().GetInventoryComponent().GetPets()));
    }
}
