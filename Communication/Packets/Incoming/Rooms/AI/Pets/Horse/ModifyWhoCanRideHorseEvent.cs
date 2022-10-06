namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets.Horse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ModifyWhoCanRideHorseEvent : IPacketEvent
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

        var PetId = packet.PopInt();

        if (!Room.GetRoomUserManager().TryGetPet(PetId, out var Pet))
        {
            return;
        }

        if (Pet.PetData == null || Pet.PetData.OwnerId != session.GetUser().Id || Pet.PetData.Type != 13)
        {
            return;
        }

        if (Pet.PetData.AnyoneCanRide)
        {
            Pet.PetData.AnyoneCanRide = false;
        }
        else
        {
            Pet.PetData.AnyoneCanRide = true;
        }

        if (!Pet.PetData.AnyoneCanRide)
        {
            if (Pet.RidingHorse)
            {
                Pet.RidingHorse = false;
                var User = Room.GetRoomUserManager().GetRoomUserByVirtualId(Pet.HorseID);
                if (User != null)
                {
                    if (Room.CheckRights(User.GetClient(), true))
                    {
                        User.RidingHorse = false;
                        User.HorseID = 0;
                        User.ApplyEffect(-1);
                        User.MoveTo(User.X + 1, User.Y + 1);
                    }
                }
            }
        }


        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            BotPetDao.UpdateAnyoneRide(dbClient, PetId, Pet.PetData.AnyoneCanRide);
        }

        Room.SendPacket(new PetInformationComposer(Pet.PetData, Pet.RidingHorse));
    }
}
