namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Games.GameClients;

internal class GetPetInformationEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null || session.GetUser().CurrentRoom == null)
        {
            return;
        }

        var petId = packet.PopInt();

        if (!session.GetUser().CurrentRoom.RoomUserManager.TryGetPet(petId, out var pet))
        {
            var user = session.GetUser().CurrentRoom.RoomUserManager.GetRoomUserByUserId(petId);
            if (user == null)
            {
                return;
            }

            if (user.Client == null || user.Client.GetUser() == null)
            {
                return;
            }

            session.SendPacket(new PetInformationComposer(user.Client.GetUser()));
            return;
        }

        if (pet.RoomId != session.GetUser().CurrentRoomId || pet.PetData == null)
        {
            return;
        }

        session.SendPacket(new PetInformationComposer(pet.PetData, pet.RidingHorse));
    }
}
