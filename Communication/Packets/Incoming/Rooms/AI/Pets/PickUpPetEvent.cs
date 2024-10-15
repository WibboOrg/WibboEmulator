namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class PickUpPetEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.InRoom)
        {
            return;
        }

        if (Session == null || Session.User == null || Session.User.InventoryComponent == null)
        {
            return;
        }


        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        var petId = packet.PopInt();

        if (!room.RoomUserManager.TryGetPet(petId, out var pet))
        {
            if ((!room.CheckRights(Session) && room.RoomData.WhoCanKick != 2 && room.RoomData.Group == null) || (room.RoomData.Group != null && !room.CheckRights(Session)))
            {
                return;
            }

            var TargetUser = Session.User.Room.RoomUserManager.GetRoomUserByUserId(petId);
            if (TargetUser == null)
            {
                return;
            }

            if (TargetUser.Client == null || TargetUser.Client.User == null)
            {
                return;
            }

            TargetUser.IsTransf = false;

            room.SendPacket(new UserRemoveComposer(TargetUser.VirtualId));

            room.SendPacket(new UsersComposer(TargetUser));
            return;
        }

        if (Session.User.Id != pet.PetData.OwnerId && !room.CheckRights(Session, true))
        {
            return;
        }

        if (pet.RidingHorse)
        {
            var userRiding = room.RoomUserManager.GetRoomUserByVirtualId(pet.HorseID);
            if (userRiding != null)
            {
                userRiding.RidingHorse = false;
                userRiding.ApplyEffect(-1);
                userRiding.MoveTo(new Point(userRiding.X + 1, userRiding.Y + 1));
            }
            else
            {
                pet.RidingHorse = false;
            }
        }

        var petData = pet.PetData;

        pet.RoomId = 0;
        petData.PlacedInRoom = false;

        using (var dbClient = DatabaseManager.Connection)
        {
            BotPetDao.UpdateRoomId(dbClient, petData.PetId, 0);
        }

        if (petData.OwnerId != Session.User.Id)
        {
            var target = GameClientManager.GetClientByUserID(petData.OwnerId);
            if (target != null)
            {
                _ = target.User.InventoryComponent.TryAddPet(pet.PetData);
                room.RoomUserManager.RemoveBot(pet.VirtualId, false);

                target.SendPacket(new PetInventoryComposer(target.User.InventoryComponent.Pets));
                return;
            }
        }
        else
        {
            _ = Session.User.InventoryComponent.TryAddPet(pet.PetData);
            room.RoomUserManager.RemoveBot(pet.VirtualId, false);
            Session.SendPacket(new PetInventoryComposer(Session.User.InventoryComponent.Pets));
        }
    }
}
