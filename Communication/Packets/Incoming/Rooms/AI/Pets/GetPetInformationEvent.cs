namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Games.GameClients;

internal sealed class GetPetInformationEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null || Session.User.Room == null)
        {
            return;
        }

        var petId = packet.PopInt();

        if (!Session.User.Room.RoomUserManager.TryGetPet(petId, out var pet))
        {
            var user = Session.User.Room.RoomUserManager.GetRoomUserByUserId(petId);
            if (user == null)
            {
                return;
            }

            if (user.Client == null || user.Client.User == null)
            {
                return;
            }

            Session.SendPacket(new PetInformationComposer(user.Client.User));
            return;
        }

        if (pet.RoomId != Session.User.RoomId || pet.PetData == null)
        {
            return;
        }

        Session.SendPacket(new PetInformationComposer(pet.PetData, pet.RidingHorse));
    }
}
