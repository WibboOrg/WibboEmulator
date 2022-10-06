namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.AI;

internal class PlacePetEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var Room))
        {
            return;
        }

        if (!Room.CheckRights(session, true))
        {
            //session.SendPacket(new RoomErrorNotifComposer(1));
            return;
        }

        if (Room.GetRoomUserManager().BotPetCount >= 30)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.placepet.error", session.Langue));
            return;
        }

        if (!session.GetUser().GetInventoryComponent().TryGetPet(Packet.PopInt(), out var Pet))
        {
            return;
        }

        if (Pet == null)
        {
            return;
        }

        if (Pet.PlacedInRoom)
        {
            return;
        }

        var X = Packet.PopInt();
        var Y = Packet.PopInt();

        if (!Room.GetGameMap().CanWalk(X, Y, false))
        {
            //session.SendPacket(new RoomErrorNotifComposer(4));
            return;
        }

        if (Room.GetRoomUserManager().TryGetPet(Pet.PetId, out var OldPet))
        {
            Room.GetRoomUserManager().RemoveBot(OldPet.VirtualId, false);
        }

        Pet.X = X;
        Pet.Y = Y;

        Pet.PlacedInRoom = true;
        Pet.RoomId = Room.Id;

        Room.GetRoomUserManager().DeployBot(new RoomBot(Pet.PetId, Pet.OwnerId, Pet.RoomId, BotAIType.Pet, true, Pet.Name, "", "", Pet.Look, X, Y, 0, 0, false, "", 0, false, 0, 0, 0), Pet);

        Pet.DBState = DatabaseUpdateState.NEEDS_UPDATE;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            BotPetDao.UpdateRoomId(dbClient, Pet.PetId, Pet.RoomId);
        }

        if (!session.GetUser().GetInventoryComponent().TryRemovePet(Pet.PetId, out var ToRemove))
        {
            Console.WriteLine("Error whilst removing pet: " + ToRemove.PetId);
            return;
        }

        session.SendPacket(new PetInventoryComposer(session.GetUser().GetInventoryComponent().GetPets()));
    }
}
