namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Games.GameClients;

internal sealed class GetPetTrainingPanelEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null || !Session.User.InRoom)
        {
            return;
        }

        var petId = packet.PopInt();

        if (!Session.User.Room.RoomUserManager.TryGetPet(petId, out var pet))
        {
            return;
        }

        //Continue as a regular pet..
        if (pet.RoomId != Session.User.RoomId || pet.PetData == null)
        {
            return;
        }

        Session.SendPacket(new PetTrainingPanelComposer(pet.PetData));
    }
}
