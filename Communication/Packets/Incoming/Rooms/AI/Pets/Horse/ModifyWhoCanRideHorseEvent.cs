namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets.Horse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.GameClients;

internal class ModifyWhoCanRideHorseEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().InRoom)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        var petId = packet.PopInt();

        if (!room.RoomUserManager.TryGetPet(petId, out var pet))
        {
            return;
        }

        if (pet.PetData == null || pet.PetData.OwnerId != session.GetUser().Id || pet.PetData.Type != 13)
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


        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            BotPetDao.UpdateAnyoneRide(dbClient, petId, pet.PetData.AnyoneCanRide);
        }

        room.SendPacket(new PetInformationComposer(pet.PetData, pet.RidingHorse));
    }
}
