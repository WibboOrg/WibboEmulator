namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Games.GameClients;

internal sealed class GetPetInformationEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null || session.User.CurrentRoom == null)
        {
            return;
        }

        var petId = packet.PopInt();

        if (!session.User.CurrentRoom.RoomUserManager.TryGetPet(petId, out var pet))
        {
            var user = session.User.CurrentRoom.RoomUserManager.GetRoomUserByUserId(petId);
            if (user == null)
            {
                return;
            }

            if (user.Client == null || user.Client.User == null)
            {
                return;
            }

            session.SendPacket(new PetInformationComposer(user.Client.User));
            return;
        }

        if (pet.RoomId != session.User.CurrentRoomId || pet.PetData == null)
        {
            return;
        }

        session.SendPacket(new PetInformationComposer(pet.PetData, pet.RidingHorse));
    }
}
