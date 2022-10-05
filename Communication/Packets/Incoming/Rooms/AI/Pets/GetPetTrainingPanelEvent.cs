namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Games.GameClients;

internal class GetPetTrainingPanelEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null || session.GetUser() == null || !session.GetUser().InRoom)
        {
            return;
        }

        var PetId = Packet.PopInt();

        if (!session.GetUser().CurrentRoom.GetRoomUserManager().TryGetPet(PetId, out var Pet))
        {
            return;
        }

        //Continue as a regular pet..
        if (Pet.RoomId != session.GetUser().CurrentRoomId || Pet.PetData == null)
        {
            return;
        }

        session.SendPacket(new PetTrainingPanelComposer(Pet.PetData));
    }
}