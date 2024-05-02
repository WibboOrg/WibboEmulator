namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets.Horse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ModifyWhoCanRideHorseEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.InRoom)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        var petId = packet.PopInt();

        if (!room.RoomUserManager.TryGetPet(petId, out var pet))
        {
            return;
        }

        if (pet.PetData == null || pet.PetData.OwnerId != session.User.Id || pet.PetData.Type != 13)
        {
            return;
        }

        if (pet.PetData.AnyoneCanRide)
        {
            pet.PetData.AnyoneCanRide = false;
        }
        else
        {
            pet.PetData.AnyoneCanRide = true;
        }

        if (!pet.PetData.AnyoneCanRide)
        {
            if (pet.RidingHorse)
            {
                pet.RidingHorse = false;
                var user = room.RoomUserManager.GetRoomUserByVirtualId(pet.HorseID);
                if (user != null)
                {
                    if (room.CheckRights(user.Client, true))
                    {
                        user.RidingHorse = false;
                        user.HorseID = 0;
                        user.ApplyEffect(-1);
                        user.MoveTo(user.X + 1, user.Y + 1);
                    }
                }
            }
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            BotPetDao.UpdateAnyoneRide(dbClient, petId, pet.PetData.AnyoneCanRide);
        }

        room.SendPacket(new PetInformationComposer(pet.PetData, pet.RidingHorse));
    }
}
