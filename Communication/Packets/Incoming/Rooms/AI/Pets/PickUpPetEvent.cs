namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Pets;
using WibboEmulator.Games.Rooms;

internal class PickUpPetEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!session.GetUser().InRoom)
        {
            return;
        }

        if (session == null || session.GetUser() == null || session.GetUser().GetInventoryComponent() == null)
        {
            return;
        }


        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var Room))
        {
            return;
        }

        var PetId = Packet.PopInt();

        if (!Room.GetRoomUserManager().TryGetPet(PetId, out var Pet))
        {
            //Check kick rights, just because it seems most appropriate.
            if ((!Room.CheckRights(session) && Room.RoomData.WhoCanKick != 2 && Room.RoomData.Group == null) || (Room.RoomData.Group != null && !Room.CheckRights(session)))
            {
                return;
            }

            //Okay so, we've established we have no pets in this room by this virtual Id, let us check out users, maybe they're creeping as a pet?!
            var TargetUser = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(PetId);
            if (TargetUser == null)
            {
                return;
            }

            //Check some values first, please!
            if (TargetUser.GetClient() == null || TargetUser.GetClient().GetUser() == null)
            {
                return;
            }

            TargetUser.IsTransf = false;

            //Quickly remove the old user instance.
            Room.SendPacket(new UserRemoveComposer(TargetUser.VirtualId));

            //Add the new one, they won't even notice a thing!!11 8-)
            Room.SendPacket(new UsersComposer(TargetUser));
            return;
        }

        if (session.GetUser().Id != Pet.PetData.OwnerId && !Room.CheckRights(session, true))
        {
            return;
        }

        if (Pet.RidingHorse)
        {
            var UserRiding = Room.GetRoomUserManager().GetRoomUserByVirtualId(Pet.HorseID);
            if (UserRiding != null)
            {
                UserRiding.RidingHorse = false;
                UserRiding.ApplyEffect(-1);
                UserRiding.MoveTo(new Point(UserRiding.X + 1, UserRiding.Y + 1));
            }
            else
            {
                Pet.RidingHorse = false;
            }
        }

        var pet = Pet.PetData;

        pet.RoomId = 0;
        pet.PlacedInRoom = false;

        pet.DBState = DatabaseUpdateState.NEEDS_UPDATE;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            BotPetDao.UpdateRoomId(dbClient, pet.PetId, 0);
        }

        if (pet.OwnerId != session.GetUser().Id)
        {
            var Target = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(pet.OwnerId);
            if (Target != null)
            {
                Target.GetUser().GetInventoryComponent().TryAddPet(Pet.PetData);
                Room.GetRoomUserManager().RemoveBot(Pet.VirtualId, false);

                Target.SendPacket(new PetInventoryComposer(Target.GetUser().GetInventoryComponent().GetPets()));
                return;
            }
        }
        else
        {
            session.GetUser().GetInventoryComponent().TryAddPet(Pet.PetData);
            Room.GetRoomUserManager().RemoveBot(Pet.VirtualId, false);
            session.SendPacket(new PetInventoryComposer(session.GetUser().GetInventoryComponent().GetPets()));
        }
    }
}
