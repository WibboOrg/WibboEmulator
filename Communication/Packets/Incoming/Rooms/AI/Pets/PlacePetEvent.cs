using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Pets;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Rooms.AI;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class PlacePetEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                //Session.SendPacket(new RoomErrorNotifComposer(1));
                return;
            }

            if (Room.GetRoomUserManager().BotPetCount >= 30)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.placepet.error", Session.Langue));
                return;
            }

            if (!Session.GetUser().GetInventoryComponent().TryGetPet(Packet.PopInt(), out Pet Pet))
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

            int X = Packet.PopInt();
            int Y = Packet.PopInt();

            if (!Room.GetGameMap().CanWalk(X, Y, false))
            {
                //Session.SendPacket(new RoomErrorNotifComposer(4));
                return;
            }

            if (Room.GetRoomUserManager().TryGetPet(Pet.PetId, out RoomUser OldPet))
            {
                Room.GetRoomUserManager().RemoveBot(OldPet.VirtualId, false);
            }

            Pet.X = X;
            Pet.Y = Y;

            Pet.PlacedInRoom = true;
            Pet.RoomId = Room.Id;

            Room.GetRoomUserManager().DeployBot(new RoomBot(Pet.PetId, Pet.OwnerId, Pet.RoomId, BotAIType.Pet, true, Pet.Name, "", "", Pet.Look, X, Y, 0, 0, false, "", 0, false, 0, 0, 0), Pet);
            
            Pet.DBState = DatabaseUpdateState.NEEDS_UPDATE;

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.UpdateRoomId(dbClient, Pet.PetId, Pet.RoomId);
            }

            if (!Session.GetUser().GetInventoryComponent().TryRemovePet(Pet.PetId, out Pet ToRemove))
            {
                Console.WriteLine("Error whilst removing pet: " + ToRemove.PetId);
                return;
            }

            Session.SendPacket(new PetInventoryComposer(Session.GetUser().GetInventoryComponent().GetPets()));
        }
    }
}
