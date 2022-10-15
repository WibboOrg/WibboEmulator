namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Games.GameClients;

internal class GetPetTrainingPanelEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null || !session.User.InRoom)
        {
            return;
        }

        var petId = packet.PopInt();

        if (!session.User.CurrentRoom.RoomUserManager.TryGetPet(petId, out var pet))
        {
            return;
        }

        //Continue as a regular pet..
        if (pet.RoomId != session.User.CurrentRoomId || pet.PetData == null)
        {
            return;
        }

        session.SendPacket(new PetTrainingPanelComposer(pet.PetData));
    }
}
